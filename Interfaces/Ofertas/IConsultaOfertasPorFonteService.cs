using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IConsultaOfertasPorFonteService
{
    Task<CommandResult<List<OfertaResumoDto>>> ConsultarAsync(string fonte, CancellationToken cancellationToken);
}