using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Models;
using plataforma.ofertas.Storage;
using Supabase.Postgrest;
using Client = Supabase.Client;

namespace plataforma.ofertas.Repositories;

public class OfertaRepository(SupabaseContext context) : IOfertaRepository
{
    private readonly Client _client = context.Client;

    public async Task<Guid> CadastrarAsync(Oferta oferta, CancellationToken ct)
    {
        var response = await _client.From<Oferta>().Insert(oferta, cancellationToken: ct);
        return response.Models.First().Id;
    }

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

    public async Task<List<Oferta>> ObterRecentesAsync(CancellationToken ct)
    {
        var response = await _client
            .From<Oferta>()
            .Order("publicado_em", Constants.Ordering.Descending)
            .Get(ct);

        return response.Models;
    }

    public async Task<Oferta> ObterPorIdAsync(Guid id, CancellationToken ct)
    {
        var response = await _client
            .From<Oferta>()
            .Filter("id", Constants.Operator.Equals, id.ToString())
            .Get(cancellationToken: ct);

        return response.Models.FirstOrDefault();
    }

    public async Task<List<string>> ObterLinksExistentesAsync(CancellationToken ct)
    {
        var response = await _client
            .From<Oferta>()
            .Select("link")
            .Get(cancellationToken: ct);

        return response.Models.Select(o => o.Link).ToList();
    }

    public async Task<bool> DeletarAsync(Guid id, CancellationToken ct)
    {
        await _client
            .From<Oferta>()
            .Filter("id", Constants.Operator.Equals, id.ToString())
            .Delete(cancellationToken: ct);

        return true;
    }
    
    public async Task AtualizarAsync(Oferta oferta, CancellationToken ct)
    {
        await _client
            .From<Oferta>()
            .Where(o => o.Id == oferta.Id)
            .Update(oferta, cancellationToken: ct);
    }
}