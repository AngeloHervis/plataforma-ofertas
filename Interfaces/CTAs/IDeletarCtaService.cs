using plataforma.ofertas._Base;

namespace plataforma.ofertas.Interfaces.CTAs;

public interface IDeletarCtaService
{
    Task<CommandResult<Guid>> DeletarAsync(Guid ctaId, CancellationToken cancellationToken);
}