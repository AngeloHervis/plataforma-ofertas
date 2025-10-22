using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Models;
using plataforma.ofertas.Storage;
using Supabase.Postgrest;
using Client = Supabase.Client;

namespace plataforma.ofertas.Repositories;

public class OfertaRepository(SupabaseContext context) : IOfertaRepository
{
    private readonly Client _client = context.Client;

    public async Task SalvarVariasAsync(List<Oferta> ofertas, CancellationToken ct)
    {
        if (ofertas is null || ofertas.Count == 0)
            return;
        
        var ofertasUnicas = ofertas
            .GroupBy(o => o.Link)
            .Select(g => g.First())
            .ToList();

        await _client.From<Oferta>().Upsert(ofertasUnicas, cancellationToken: ct);
    }

    public async Task<List<Oferta>> ObterRecentesAsync(int limite, CancellationToken ct)
    {
        var response = await _client
            .From<Oferta>()
            .Order("publicado_em", Constants.Ordering.Descending)
            .Limit(limite)
            .Get(ct);

        return response.Models;
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct)
    {
        await _client
            .From<Oferta>()
            .Filter("id", Constants.Operator.Equals, id)
            .Delete(cancellationToken: ct);
    }
}