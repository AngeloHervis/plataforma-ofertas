using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Templates;

namespace plataforma.ofertas.Interfaces.Templates;

public interface ICriarTemplateService
{
    Task<CommandResult<Guid>> CriarAsync(CriarTemplateDto criarTemplateDto, CancellationToken cancellationToken);
}