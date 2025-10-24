using plataforma.ofertas.Interfaces.Jobs;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Interfaces.Scrapers;

namespace plataforma.ofertas.Services.Jobs;

public class AtualizarOfertasJob(
    IPelandoScraperService pelando,
    IPromobitScraperService promobit,
    IMercadoLivreScraperService meli,
    IOfertaRepository repo
) : IRunnableService
{
    public string Name => "atualizar-ofertas";

    public async Task RunAsync(CancellationToken ct)
    {
        return;
        
        var resultados = await Task.WhenAll(
            pelando.ScrapePelandoAsync(ct),
            promobit.ScrapePromobitAsync(ct),
            meli.ObterInformacoesCompletasMercadoLivreAsync(ct)
        );

        var ofertasValidas = resultados
            .Where(r => !r.IsError && r.Data is not null)
            .SelectMany(r => r.Data!)
            .DistinctBy(o => new { o.Titulo, o.Link })
            .ToList();

        if (ofertasValidas.Count == 0) return;

        await repo.SalvarVariasAsync(ofertasValidas, ct);
    }
}