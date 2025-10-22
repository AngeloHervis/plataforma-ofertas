using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IConsultaOfertasRecentesService
{
    Task<CommandResult<List<OfertaResumoDto>>> ConsultarAsync(CancellationToken cancellationToken);
}