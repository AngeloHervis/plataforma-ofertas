using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.CTAs;

namespace plataforma.ofertas.Interfaces.CTAs;

public interface IConsultaCtasService
{
    Task<CommandResult<List<CtaDetalhesDto>>> ConsultarAsync(CancellationToken cancellationToken);
}