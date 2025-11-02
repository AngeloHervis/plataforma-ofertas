using plataforma.ofertas._Base;
using plataforma.ofertas.Interfaces.Templates;

namespace plataforma.ofertas.Services.Templates;

public class DeletarTemplateService(ITemplateRepository templateRepository) : IDeletarTemplateService
{
    public async Task<CommandResult<Guid>> DeletarAsync(Guid TemplateId, CancellationToken cancellationToken)
    {
        var template = await templateRepository.ObterPorIdAsync(TemplateId, cancellationToken);
        if (template == null)
            return CommandResult<Guid>.NotFound("Template não encontrada.");

        await templateRepository.DeletarAsync(TemplateId, cancellationToken);
        return CommandResult<Guid>.Success(template.Id);
    }
}