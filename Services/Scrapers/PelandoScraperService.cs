using HtmlAgilityPack;
using plataforma.ofertas._Base;
using plataforma.ofertas.Interfaces.Scrapers;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Scrapers;

public class PelandoScraperService(
    HttpClient httpClient,
    IAmazonScraperService amazonScraperService,
    IShopeeScraperService shopeeScraperService
) : IPelandoScraperService
{
    public async Task<CommandResult<List<Oferta>>> ScrapePelandoAsync(CancellationToken ct)
    {
        try
        {
            var conteudoPagina = await httpClient.GetStringAsync(UrlRecentesPelando, ct);
            if (string.IsNullOrWhiteSpace(conteudoPagina))
                return CommandResult<List<Oferta>>.ExternalError(
                    "Não foi possível obter o conteúdo da página do Pelando");

            var documento = new HtmlDocument();
            documento.LoadHtml(conteudoPagina);

            var cartoesDeal = documento.DocumentNode.SelectNodes(SeletorCartaoDeal);
            if (cartoesDeal == null || cartoesDeal.Count == 0)
                return CommandResult<List<Oferta>>.NotFound("Nenhum cartão de deal encontrado na página");

            var ofertas = new List<Oferta>();
            await BuscarOfertasAsync(cartoesDeal, ofertas, ct);

            return CommandResult<List<Oferta>>.Success(ofertas);
        }
        catch (Exception ex)
        {
            return CommandResult<List<Oferta>>.InternalError($"Erro interno ao processar Pelando: {ex.Message}");
        }
    }

    private async Task BuscarOfertasAsync(HtmlNodeCollection cartoesDeal, List<Oferta> ofertas, CancellationToken ct)
    {
        foreach (var cartao in cartoesDeal)
        {
            var noHref = cartao.SelectSingleNode(SeletorLink);
            if (noHref == null) continue;

            var href = noHref.GetAttributeValue("href", "");
            var linkPromocao = href.StartsWith("/") ? UrlBasePelando + href : href;
            if (string.IsNullOrWhiteSpace(linkPromocao)) continue;

            var (nomeLoja, urlLoja) = ExtrairInformacoesDaLoja(cartao);
            if (!EhLojaPermitida((nomeLoja, urlLoja))) continue;

            await ObterProdutosAsync(ofertas, nomeLoja, urlLoja, linkPromocao, ct);
        }
    }

    private static (string NomeLoja, string UrlLoja) ExtrairInformacoesDaLoja(HtmlNode cartao)
    {
        var nomeLoja = cartao.SelectSingleNode(SeletorLoja)?.InnerText?.Trim() ?? string.Empty;

        var urlLoja = string.Empty;
        var linkNode = cartao.SelectSingleNode(".//a[contains(@href,'amazon.') or contains(@href,'shopee.')]");
        if (linkNode != null)
        {
            urlLoja = linkNode.GetAttributeValue("href", "");
        }

        return (nomeLoja, urlLoja);
    }

    private async Task ObterProdutosAsync(List<Oferta> ofertas, string nomeLoja, string urlLoja, string linkPromocao,
        CancellationToken ct)
    {
        if (nomeLoja.Contains("Amazon", StringComparison.OrdinalIgnoreCase) || urlLoja.Contains("amazon."))
        {
            await ObterProdutoAmazon(linkPromocao, ofertas, ct);
            return;
        }

        if (nomeLoja.Contains("Shopee", StringComparison.OrdinalIgnoreCase) || urlLoja.Contains("shopee."))
        {
            await ObterProdutoShopee(linkPromocao, ofertas, ct);
            return;
        }
    }

    private async Task ObterProdutoAmazon(string linkPromocao, List<Oferta> ofertas, CancellationToken ct)
    {
        var linkRealAmazon = await ObterLinkRealAmazonDoPelando(linkPromocao, ct);
        if (string.IsNullOrEmpty(linkRealAmazon)) return;
        
        var fonte = "Pelando";
        var infoAmazon = await amazonScraperService.ObterInformacoesCompletasDaAmazonAsync(linkRealAmazon, fonte, ct);
        if (infoAmazon.IsValid)
        {
            var ofertaAmazon = new Oferta
            {
                Id = Guid.NewGuid(),
                Fonte = fonte,
                Titulo = infoAmazon.Titulo,
                Link = infoAmazon.Link,
                ImagemUrl = infoAmazon.ImagemUrl,
                PrecoAtual = infoAmazon.PrecoAtual,
                PrecoAnterior = infoAmazon.PrecoAnterior,
                DescontoPercentual = CalcularPercentual(infoAmazon.PrecoAtual, infoAmazon.PrecoAnterior),
                PublicadoEm = DateTime.UtcNow
            };

            ofertas.Add(ofertaAmazon);
        }
    }

    private async Task<string> ObterLinkRealAmazonDoPelando(string linkPromocao, CancellationToken ct)
    {
        try
        {
            var html = await httpClient.GetStringAsync(linkPromocao, ct);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var botao = doc.DocumentNode.SelectSingleNode(SeletorBotaoLoja);
            if (botao == null) return null;

            var urlRedirect = botao.GetAttributeValue("href", null);
            if (string.IsNullOrEmpty(urlRedirect)) return null;

            using var redirectHandler = new HttpClientHandler();
            redirectHandler.AllowAutoRedirect = true;
            redirectHandler.MaxAutomaticRedirections = 10;

            using var redirectClient = new HttpClient(redirectHandler);
            var response = await redirectClient.GetAsync(urlRedirect, HttpCompletionOption.ResponseHeadersRead, ct);
            var linkFinal = response.RequestMessage?.RequestUri?.ToString();
            return linkFinal;
        }
        catch
        {
            return null;
        }
    }

    private async Task ObterProdutoShopee(string linkPromocao, List<Oferta> ofertas, CancellationToken ct)
    {
        var infoShopee = await shopeeScraperService.ObterInformacoesCompletasDaShopeeAsync(linkPromocao, ct);
        if (infoShopee.IsValid)
        {
            var ofertaShopee = new Oferta
            {
                Id = Guid.NewGuid(),
                Fonte = "Pelando",
                Titulo = infoShopee.Titulo,
                Link = infoShopee.Link,
                ImagemUrl = infoShopee.ImagemUrl,
                PrecoAtual = infoShopee.PrecoAtual,
                PrecoAnterior = infoShopee.PrecoAnterior,
                DescontoPercentual = CalcularPercentual(infoShopee.PrecoAtual, infoShopee.PrecoAnterior),
                PublicadoEm = DateTime.UtcNow
            };

            ofertas.Add(ofertaShopee);
        }
    }

    private static bool EhLojaPermitida((string NomeLoja, string UrlLoja) infoLoja)
    {
        if (string.IsNullOrEmpty(infoLoja.NomeLoja) && string.IsNullOrEmpty(infoLoja.UrlLoja)) return false;

        var ehNomePermitido = !string.IsNullOrEmpty(infoLoja.NomeLoja) &&
                              LojasPermitidas.Any(loja =>
                                  infoLoja.NomeLoja.Contains(loja, StringComparison.OrdinalIgnoreCase));

        var ehUrlPermitida = !string.IsNullOrEmpty(infoLoja.UrlLoja) &&
                             (infoLoja.UrlLoja.Contains("amazon.") || infoLoja.UrlLoja.Contains("shopee."));

        return ehNomePermitido || ehUrlPermitida;
    }
    
    private static int? CalcularPercentual(string precoAtual, string precoAnterior)
    {
        if (string.IsNullOrEmpty(precoAnterior))
            return null;

        var precoAtualLimpo = precoAtual.Replace("R$", "").Replace(".", "").Replace(",", "");
        var precoAnteriorLimpo = precoAnterior.Replace("R$", "").Replace(".", "").Replace(",", "");

        if (!int.TryParse(precoAtualLimpo, out var precoAtualCentavos) ||
            !int.TryParse(precoAnteriorLimpo, out var precoAnteriorCentavos) ||
            precoAnteriorCentavos <= 0)
            return null;

        var desconto = ((precoAnteriorCentavos - precoAtualCentavos) * 100) / precoAnteriorCentavos;
        return desconto;
    }

    private const string SeletorBotaoLoja = "//a[contains(@class,'store-link-button')]";
    private const string UrlBasePelando = "https://www.pelando.com.br";
    private const string UrlRecentesPelando = "https://www.pelando.com.br/recentes";
    private const string SeletorCartaoDeal = "//li//div[contains(@class,'_deal-card_')]";
    private const string SeletorLoja = ".//div[contains(@class,'_default-deal-card-store_')]";
    private const string SeletorLink = ".//a[contains(@href,'/d/')]";
    private static readonly string[] LojasPermitidas = ["Amazon", "Shopee"];
}