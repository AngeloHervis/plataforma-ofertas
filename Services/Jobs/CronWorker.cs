using Cronos;

namespace plataforma.ofertas.Services.Jobs;

public class CronWorker(IJobRegistry registry, ILogger<CronWorker> logger, IServiceProvider sp) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = registry.Jobs.Select(cfg => RunLoopAsync(cfg, stoppingToken)).ToArray();
        await Task.WhenAll(tasks);
    }

    private async Task RunLoopAsync(CronJobConfig cfg, CancellationToken ct)
    {
        var job = registry.Resolve(cfg.Service);
        if (job is null) return;

        var cron = CronExpression.Parse(cfg.Cron, CronFormat.Standard);
        DateTimeOffset? next = cron.GetNextOccurrence(DateTime.UtcNow);
        while (!ct.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            if (next is null)
            {
                next = cron.GetNextOccurrence(now);
                continue;
            }

            var delay = next.Value - now;
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, ct);

            try
            {
                using var scope = sp.CreateScope();
                await job.RunAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha ao executar job {Job}", job.Name);
            }

            next = cron.GetNextOccurrence(DateTime.UtcNow);
        }
    }
}