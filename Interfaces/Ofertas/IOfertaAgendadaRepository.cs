using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IOfertaAgendadaRepository
{
    Task<OfertaAgendada> CriarAsync(OfertaAgendada entidade, CancellationToken ct);
    Task<List<OfertaAgendada>> ListarTodasAsync(CancellationToken ct);
    Task<OfertaAgendada> ObterPorIdAsync(Guid id, CancellationToken ct);
}