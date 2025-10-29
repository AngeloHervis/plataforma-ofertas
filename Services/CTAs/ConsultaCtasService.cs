using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.CTAs;
using plataforma.ofertas.Interfaces.CTAs;

namespace plataforma.ofertas.Services.CTAs;

public class ConsultaCtasService(ICtaRepository ctaRepository) : IConsultaCtasService
{
    public async Task<CommandResult<List<CtaDetalhesDto>>> ConsultarAsync(CancellationToken cancellationToken)
    {
        var ctas = await ctaRepository.ObterCtasAsync(cancellationToken);
        var dto = ctas.Select(cta => new CtaDetalhesDto
        {
            Id = cta.Id,
            Titulo = cta.Titulo
        }).ToList();
        
        return CommandResult<List<CtaDetalhesDto>>.Success(dto);
    }
}