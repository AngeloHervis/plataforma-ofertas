using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Templates;
using plataforma.ofertas.Interfaces.Templates;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Templates;

public class CriarTemplateService(ITemplateRepository templateRepository) : ICriarTemplateService
{
    public async Task<CommandResult<Guid>> CriarAsync(CriarTemplateDto criarTemplateDto, CancellationToken cancellationToken)
    {
        var novaTemplate = new Template
        {
            Nome = criarTemplateDto.Nome,
            Conteudo = criarTemplateDto.Conteudo,
            Descricao = criarTemplateDto.Descricao
        };
        
        var id = await templateRepository.AdicionarAsync(novaTemplate, cancellationToken);
        
        return CommandResult<Guid>.Success(novaTemplate.Id);
    }
}