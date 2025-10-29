using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.CTAs;

namespace plataforma.ofertas.Interfaces.CTAs;

public interface ICriarCtaService
{
    Task<CommandResult<Guid>> CriarAsync(CriarCtaDto criarCtaDto, CancellationToken cancellationToken);
}