using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Templates;

namespace plataforma.ofertas.Interfaces.Templates;

public interface IConsultaTemplatesService
{
    Task<CommandResult<List<TemplateDetalhesDto>>> ConsultarAsync(CancellationToken cancellationToken);
}