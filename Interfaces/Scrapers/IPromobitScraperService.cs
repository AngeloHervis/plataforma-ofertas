using plataforma.ofertas._Base;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces.Scrapers;

public interface IPromobitScraperService
{
    Task<CommandResult<List<Oferta>>> ScrapePromobitAsync(CancellationToken ct);
}