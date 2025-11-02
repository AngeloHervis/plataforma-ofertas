using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Templates;
using plataforma.ofertas.Interfaces.Templates;

namespace plataforma.ofertas.Services.Templates;

public class ConsultaTemplatesService(ITemplateRepository templateRepository) : IConsultaTemplatesService
{
    public async Task<CommandResult<List<TemplateDetalhesDto>>> ConsultarAsync(CancellationToken cancellationToken)
    {
        var templates = await templateRepository.ObterTemplatesAsync(cancellationToken);
        var dto = templates.Select(template => new TemplateDetalhesDto
        {
            Id = template.Id,
            Conteudo = template.Conteudo,
            Nome = template.Nome,
            Descricao = template.Descricao
        }).ToList();
        
        return CommandResult<List<TemplateDetalhesDto>>.Success(dto);
    }
}