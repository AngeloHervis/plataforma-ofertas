using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces;

public interface IScraperApiService
{
    Task<Oferta> ObterOfertaAmazonAsync(string url, string fonte, CancellationToken ct);
}