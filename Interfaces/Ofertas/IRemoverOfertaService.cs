using plataforma.ofertas._Base;
using plataforma.ofertas.Dto._Base;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IRemoverOfertaService
{
    Task<CommandResult<RespostaCodigoBase>> RemoverAsync(Guid id, CancellationToken cancellationToken);
}