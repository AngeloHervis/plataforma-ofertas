using plataforma.ofertas._Base;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IRenovarOfertaService
{
    Task<CommandResult<Guid>> RenovarOfertaAsync(Guid ofertaId, CancellationToken cancellationToken);
}