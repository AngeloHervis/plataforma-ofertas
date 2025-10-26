using plataforma.ofertas.Interfaces.Jobs;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Interfaces.Scrapers;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Jobs;

public class AtualizarOfertasJob(
    IPelandoScraperService pelando,
    IPromobitScraperService promobit,
    IMercadoLivreScraperService meli,
    ILogger<AtualizarOfertasJob> logger,
    IOfertaRepository repo
) : IRunnableService
{
    public string Name => "atualizar-ofertas";

    public async Task RunAsync(CancellationToken ct)
    {
        logger.LogInformation("Iniciando atualização de ofertas...");

        var resultados = await Task.WhenAll(
            pelando.ScrapePelandoAsync(ct),
            promobit.ScrapePromobitAsync(ct),
            meli.ObterInformacoesCompletasListaMercadoLivreAsync(ct)
        );

        logger.LogInformation("Scraping concluído. Buscando links existentes...");

        var linksExistentes = await repo.ObterLinksExistentesAsync(ct);

        var ofertasValidas = resultados
            .Where(r => !r.IsError && r.Data is not null)
            .SelectMany(r => r.Data!)
            .Where(o => !linksExistentes.Contains(o.Link))
            .Where(ValidarOferta)
            .DistinctBy(o => new { o.Titulo, o.Link })
            .ToList();

        logger.LogInformation("Ofertas válidas encontradas: {Count}", ofertasValidas.Count);

        if (ofertasValidas.Count == 0)
        {
            logger.LogInformation("Nenhuma nova oferta válida encontrada.");
            return;
        }

        await repo.SalvarVariasAsync(ofertasValidas, ct);

        logger.LogInformation("Novas ofertas salvas com sucesso.");
    }

    private static bool ValidarOferta(Oferta oferta)
    {
        return !string.IsNullOrWhiteSpace(oferta.Titulo) &&
               !string.IsNullOrWhiteSpace(oferta.Link) &&
               !string.IsNullOrWhiteSpace(oferta.PrecoAtual) &&
               oferta.PrecoAtual != "Não encontrado";
    }
}