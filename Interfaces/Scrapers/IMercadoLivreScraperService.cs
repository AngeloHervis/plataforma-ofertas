using plataforma.ofertas._Base;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces.Scrapers;

public interface IMercadoLivreScraperService
{
    Task<CommandResult<List<Oferta>>> ObterInformacoesCompletasListaMercadoLivreAsync(CancellationToken cancellationToken);
    Task<ProductInfo> ObterInformacoesCompletasMercadoLivreAsync(string linkDeal, CancellationToken ct);
}