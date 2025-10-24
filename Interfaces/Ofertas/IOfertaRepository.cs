using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IOfertaRepository
{
    Task SalvarVariasAsync(List<Oferta> ofertas, CancellationToken ct);
    Task<List<Oferta>> ObterRecentesAsync(int limite, CancellationToken ct);
    Task<Oferta> ObterPorIdAsync(Guid id, CancellationToken ct);
    Task RemoverAsync(Guid id, CancellationToken ct);
}