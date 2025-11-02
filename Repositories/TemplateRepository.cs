using plataforma.ofertas.Interfaces.Templates;
using plataforma.ofertas.Models;
using plataforma.ofertas.Storage;
using Supabase.Postgrest;
using Client = Supabase.Client;

namespace plataforma.ofertas.Repositories;

public class TemplateRepository(SupabaseContext context) : ITemplateRepository
{
    private readonly Client _client = context.Client;

    public async Task<List<Template>> ObterTemplatesAsync(CancellationToken cancellationToken)
    {
        var response = await _client
            .From<Template>()
            .Get(cancellationToken: cancellationToken);

        return response.Models;
    }

    public async Task<Template> ObterPorIdAsync(Guid TemplateId, CancellationToken cancellationToken)
    {
        var response = await _client
            .From<Template>()
            .Filter("id", Constants.Operator.Equals, TemplateId.ToString())
            .Get(cancellationToken: cancellationToken);

        return response.Model;
    }

    public async Task DeletarAsync(Guid id, CancellationToken ct)
    {
        await _client
            .From<Template>()
            .Filter("id", Constants.Operator.Equals, id.ToString())
            .Delete(cancellationToken: ct);
    }

    public async Task<Guid> AdicionarAsync(Template Template, CancellationToken cancellationToken)
    {
        var response = await _client
            .From<Template>()
            .Insert(Template, cancellationToken: cancellationToken);
        return response.Models.First().Id;
    }
}