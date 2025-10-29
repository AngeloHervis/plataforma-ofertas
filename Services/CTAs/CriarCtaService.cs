using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.CTAs;
using plataforma.ofertas.Interfaces.CTAs;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.CTAs;

public class CriarCtaService(ICtaRepository ctaRepository) : ICriarCtaService
{
    public async Task<CommandResult<Guid>> CriarAsync(CriarCtaDto criarCtaDto, CancellationToken cancellationToken)
    {
        var novaCta = new Cta
        {
            Titulo = criarCtaDto.Titulo
        };
        
        var id = await ctaRepository.AdicionarAsync(novaCta, cancellationToken);
        
        return CommandResult<Guid>.Success(novaCta.Id);
    }
}