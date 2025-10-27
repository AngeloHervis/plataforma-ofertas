using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IOfertaRepository
{
    Task SalvarVariasAsync(List<Oferta> ofertas, CancellationToken ct);
    Task<Guid> CadastrarAsync(Oferta oferta, CancellationToken ct);
    Task<List<Oferta>> ObterRecentesAsync(CancellationToken ct);
    Task<Oferta> ObterPorIdAsync(Guid id, CancellationToken ct);
    Task AtualizarAsync(Oferta oferta, CancellationToken ct);
    Task<List<string>> ObterLinksExistentesAsync(CancellationToken ct);
    Task<bool> DeletarAsync(Guid id, CancellationToken ct);
}