using plataforma.ofertas._Base;
using plataforma.ofertas.Interfaces.CTAs;

namespace plataforma.ofertas.Services.CTAs;

public class DeletarCtaService(ICtaRepository ctaRepository) : IDeletarCtaService
{
    public async Task<CommandResult<Guid>> DeletarAsync(Guid ctaId, CancellationToken cancellationToken)
    {
        var cta = await ctaRepository.ObterPorIdAsync(ctaId, cancellationToken);
        if (cta == null)
            return CommandResult<Guid>.NotFound("CTA não encontrada.");

        await ctaRepository.DeletarAsync(ctaId, cancellationToken);
        return CommandResult<Guid>.Success(cta.Id);
    }
}