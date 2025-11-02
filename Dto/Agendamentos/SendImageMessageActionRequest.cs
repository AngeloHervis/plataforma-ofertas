namespace plataforma.ofertas.Dto.Agendamentos;

public sealed class SendImageMessageActionRequest
{
    public string accountId { get; set; } = default!;
    public string releaseId { get; set; } = default!;
    public string? caption { get; set; }
    public string url { get; set; } = default!;
    public string? scheduledTo { get; set; }
    public bool? chooseSpecificGroups { get; set; } = false;
    public List<string> groupIds { get; set; } = [];
    public object? options { get; set; }
}