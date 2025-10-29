using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces.CTAs;

public interface ICtaRepository
{
    Task<List<Cta>> ObterCtasAsync(CancellationToken cancellationToken);
    Task<Cta> ObterPorIdAsync(Guid ctaId, CancellationToken cancellationToken);
    Task DeletarAsync(Guid id, CancellationToken ct);
    Task<Guid> AdicionarAsync(Cta cta, CancellationToken cancellationToken);
}