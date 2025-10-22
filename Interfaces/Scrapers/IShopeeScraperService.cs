using plataforma.ofertas._Base;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces.Scrapers;

public interface IShopeeScraperService
{
    Task<ProductInfo> ObterInformacoesCompletasDaShopeeAsync(string linkDeal, CancellationToken ct);
}