using plataforma.ofertas._Base;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IRemoverImagemOfertaService
{
    Task<CommandResult<Guid>> RemoverAsync(Guid ofertaId, string imagemUrl, CancellationToken cancellationToken);
}