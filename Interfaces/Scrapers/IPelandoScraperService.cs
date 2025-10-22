using plataforma.ofertas._Base;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces.Scrapers;

public interface IPelandoScraperService
{
    Task<CommandResult<List<Oferta>>> ScrapePelandoAsync(CancellationToken ct);
}