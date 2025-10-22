using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IConsultaOfertaDetalheService
{
    Task<CommandResult<OfertaDetalheDto>> ConsultarAsync(Guid id, CancellationToken cancellationToken);
}