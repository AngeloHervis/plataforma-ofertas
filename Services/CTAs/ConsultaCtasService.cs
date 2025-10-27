using plataforma.ofertas._Base;
using plataforma.ofertas.Interfaces.CTAs;

namespace plataforma.ofertas.Services.CTAs;

public class ConsultaCtasService(ICtaRepository ctaRepository) : IConsultaCtasService
{
    public async Task<CommandResult<List<string>>> ConsultarAsync(CancellationToken cancellationToken)
    {
        var ctas = await ctaRepository.ObterCtasAsync(cancellationToken);
        var ctaNames = ctas.Select(cta => cta.Titulo).ToList();
        return CommandResult<List<string>>.Success(ctaNames);
    }
}