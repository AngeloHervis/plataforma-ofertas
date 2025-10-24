using Microsoft.Extensions.Options;
using plataforma.ofertas.Interfaces.Jobs;

namespace plataforma.ofertas.Services.Jobs;

public record CronJobConfig(string Service, string Cron);

public interface IJobRegistry
{
    IRunnableService Resolve(string name);
    IReadOnlyList<CronJobConfig> Jobs { get; }
}

public class JobRegistry(IEnumerable<IRunnableService> services, IOptions<List<CronJobConfig>> options) : IJobRegistry
{
    private readonly Dictionary<string, IRunnableService> _map =
        services.ToDictionary(s => s.Name, s => s, StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<CronJobConfig> Jobs => options.Value;

    public IRunnableService Resolve(string name) => _map.TryGetValue(name, out var svc) ? svc : null;
}