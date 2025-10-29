using plataforma.ofertas.Interfaces.CTAs;
using plataforma.ofertas.Models;
using plataforma.ofertas.Storage;
using Supabase.Postgrest;
using Client = Supabase.Client;

namespace plataforma.ofertas.Repositories;

public class CtaRepository(SupabaseContext context) : ICtaRepository
{
    private readonly Client _client = context.Client;

    public async Task<List<Cta>> ObterCtasAsync(CancellationToken cancellationToken)
    {
        var response = await _client
            .From<Cta>()
            .Get(cancellationToken: cancellationToken);

        return response.Models;
    }

    public async Task<Cta> ObterPorIdAsync(Guid ctaId, CancellationToken cancellationToken)
    {
        var response = await _client
            .From<Cta>()
            .Filter("id", Constants.Operator.Equals, ctaId.ToString())
            .Get(cancellationToken: cancellationToken);

        return response.Model;
    }

    public async Task DeletarAsync(Guid id, CancellationToken ct)
    {
        await _client
            .From<Cta>()
            .Filter("id", Constants.Operator.Equals, id.ToString())
            .Delete(cancellationToken: ct);
    }

    public async Task<Guid> AdicionarAsync(Cta cta, CancellationToken cancellationToken)
    {
        var response = await _client
            .From<Cta>()
            .Insert(cta, cancellationToken: cancellationToken);
        return response.Models.First().Id;
    }
}