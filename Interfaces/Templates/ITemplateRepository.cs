using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces.Templates;

public interface ITemplateRepository
{
    Task<List<Template>> ObterTemplatesAsync(CancellationToken cancellationToken);
    Task<Template> ObterPorIdAsync(Guid TemplateId, CancellationToken cancellationToken);
    Task DeletarAsync(Guid id, CancellationToken ct);
    Task<Guid> AdicionarAsync(Template Template, CancellationToken cancellationToken);
}