using Microsoft.Extensions.Options;
using plataforma.ofertas.Dto.Agendamentos;
using plataforma.ofertas.Interfaces.Agendamentos;

namespace plataforma.ofertas.Services.Agendamentos;

public sealed class SendFlowActionsClient(HttpClient http, IOptions<SendFlowOptions> opts) : ISendFlowActionsClient
{
    private readonly SendFlowOptions _opts = opts.Value;

    public async Task<bool> ScheduleImageToReleaseAsync(
        string caption,
        string imageUrl,
        DateTimeOffset scheduledAtUtc,
        CancellationToken ct)
    {
        var body = new SendImageMessageActionRequest
        {
            accountId = _opts.AccountId,
            releaseId = _opts.ReleaseId,
            caption = caption,
            url = imageUrl,
            scheduledTo = scheduledAtUtc.ToUniversalTime().ToString("o"), 
            chooseSpecificGroups = false,
            options = new { shippingSpeed = _opts.DefaultShipping }
        };

        using var resp = await http.PostAsJsonAsync("actions/send-image-message", body, ct);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadFromJsonAsync<Dictionary<string, object>>(cancellationToken: ct);
        return json is not null && json.TryGetValue("actionId", out var idObj) && idObj != null;
    }
}