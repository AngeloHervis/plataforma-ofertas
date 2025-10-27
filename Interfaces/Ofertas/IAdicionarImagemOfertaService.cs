using plataforma.ofertas._Base;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IAdicionarImagemOfertaService
{
    Task<CommandResult<Guid>> AdicionarAsync(Guid ofertaId, string imagemUrl, CancellationToken cancellationToken);
}