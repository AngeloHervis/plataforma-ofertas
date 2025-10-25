namespace plataforma.ofertas.Interfaces.Agendamentos;

public interface ISendFlowActionsClient
{
    Task<bool> ScheduleImageToReleaseAsync(
        string caption,
        string imageUrl,
        DateTimeOffset scheduledAtUtc,
        CancellationToken ct);
}