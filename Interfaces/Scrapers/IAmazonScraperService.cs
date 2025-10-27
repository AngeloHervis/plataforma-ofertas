using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces.Scrapers;

public interface IAmazonScraperService
{
    Task<Oferta> ObterInformacoesCompletasDaAmazonAsync(string linkDeal, string fonte, CancellationToken ct);
}