using plataforma.ofertas._Base;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IDeletarOfertaService
{
    Task<CommandResult<Guid>> DeletarAsync(Guid id, CancellationToken ct);
}