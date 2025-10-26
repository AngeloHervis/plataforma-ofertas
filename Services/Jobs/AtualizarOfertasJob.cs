using plataforma.ofertas.Interfaces.Jobs;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Interfaces.Scrapers;
using plataforma.ofertas.Models;

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
        var resultados = await Task.WhenAll(
            pelando.ScrapePelandoAsync(ct),
            promobit.ScrapePromobitAsync(ct),
            meli.ObterInformacoesCompletasListaMercadoLivreAsync(ct)
        );

        var linksExistentes = await repo.ObterLinksExistentesAsync(ct);

        var ofertasValidas = resultados
            .Where(r => !r.IsError && r.Data is not null)
            .SelectMany(r => r.Data!)
            .Where(o => !linksExistentes.Contains(o.Link))
            .Where(ValidarOferta)
            .DistinctBy(o => new { o.Titulo, o.Link })
            .ToList();

        if (ofertasValidas.Count == 0) return;

        await repo.SalvarVariasAsync(ofertasValidas, ct);
    }

    private static bool ValidarOferta(Oferta oferta)
    {
        return !string.IsNullOrWhiteSpace(oferta.Titulo) &&
               !string.IsNullOrWhiteSpace(oferta.Link) &&
               !string.IsNullOrWhiteSpace(oferta.PrecoAtual) &&
                oferta.PrecoAtual != "Não encontrado";
    }
}