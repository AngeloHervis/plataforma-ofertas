using plataforma.ofertas._Base;

namespace plataforma.ofertas.Interfaces.Templates;

public interface IDeletarTemplateService
{
    Task<CommandResult<Guid>> DeletarAsync(Guid templateId, CancellationToken cancellationToken);
}