using plataforma.ofertas._Base;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IAtualizarTituloOfertaService
{
    Task<CommandResult<Guid>> AtualizarAsync(Guid ofertaId, string novoTitulo, CancellationToken ct);
}