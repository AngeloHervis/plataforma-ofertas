using System.Text.RegularExpressions;
using HtmlAgilityPack;
using plataforma.ofertas._Base;
using plataforma.ofertas.Interfaces.Scrapers;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Scrapers;

public class PromobitScraperService(
    HttpClient httpClient,
    IAmazonScraperService amazonScraperService
) : IPromobitScraperService
{
    public async Task<CommandResult<List<Oferta>>> ScrapePromobitAsync(CancellationToken ct)
    {
        try
        {
            var conteudoPagina = await httpClient.GetStringAsync(UrlRecentesPromobit, ct);
            if (string.IsNullOrWhiteSpace(conteudoPagina))
                return CommandResult<List<Oferta>>.ExternalError(
                    "Não foi possível obter o conteúdo da página do Promobit");

            var documento = new HtmlDocument();
            documento.LoadHtml(conteudoPagina);

            var cartoesPromocao = documento.DocumentNode.SelectNodes(SeletorCartaoPromocao);
            if (cartoesPromocao == null || cartoesPromocao.Count == 0)
                return CommandResult<List<Oferta>>.NotFound("Nenhum cartão de promoção encontrado na página");

            var ofertas = new List<Oferta>();
            await BuscarOfertasAsync(cartoesPromocao, ofertas, ct);

            return CommandResult<List<Oferta>>.Success(ofertas);
        }
        catch (Exception ex)
        {
            return CommandResult<List<Oferta>>.InternalError($"Erro interno ao processar Promobit: {ex.Message}");
        }
    }

    private async Task BuscarOfertasAsync(HtmlNodeCollection cartoesPromocao, List<Oferta> ofertas,
        CancellationToken ct)
    {
        foreach (var cartao in cartoesPromocao)
        {
            var titulo = cartao.SelectSingleNode(SeletorTitulo)?.InnerText?.Trim();
            if (string.IsNullOrWhiteSpace(titulo)) continue;

            var linkPromocao = cartao.GetAttributeValue("href", "");
            if (string.IsNullOrWhiteSpace(linkPromocao)) continue;

            if (linkPromocao.StartsWith("/"))
                linkPromocao = UrlBasePromobit + linkPromocao;

            var (nomeLoja, urlLoja) = ExtrairInformacoesDaLoja(cartao);
            if (!EhLojaPermitida((nomeLoja, urlLoja))) continue;

            await ObterProdutosAsync(ofertas, nomeLoja, urlLoja, linkPromocao, ct);
        }
    }

    private async Task ObterProdutosAsync(List<Oferta> ofertas, string nomeLoja, string urlLoja, string linkPromocao,
        CancellationToken ct)
    {
        if (nomeLoja.Contains("Amazon", StringComparison.OrdinalIgnoreCase) || urlLoja.Contains("amazon."))
            await ObterProdutoAmazon(linkPromocao, ofertas, ct);
    }

    private async Task ObterProdutoAmazon(string linkPromocao, List<Oferta> ofertas, CancellationToken ct)
    {
        var linkRealAmazon = await ObterLinkRealAmazonDoPromobit(linkPromocao, ct);
        if (string.IsNullOrEmpty(linkRealAmazon)) return;

        var fonte = "Promobit";
        var infoAmazon = await amazonScraperService.ObterInformacoesCompletasDaAmazonAsync(linkRealAmazon, fonte, ct);
        ofertas.Add(infoAmazon);
    }

    private async Task<string> ObterLinkRealAmazonDoPromobit(string linkPromocao, CancellationToken ct)
    {
        try
        {
            var html = await httpClient.GetStringAsync(linkPromocao, ct);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var linkIrALoja = doc.DocumentNode
                .SelectSingleNode("//a[contains(@href,'Redirect/to')][span[contains(text(),'Ir à loja')]]")
                ?.GetAttributeValue("href", "");

            if (string.IsNullOrEmpty(linkIrALoja))
                return null;

            var htmlLoja = await httpClient.GetStringAsync(linkIrALoja, ct);
            var docLoja = new HtmlDocument();
            docLoja.LoadHtml(htmlLoja);

            var elementoCliqueAqui = docLoja.DocumentNode
                .SelectSingleNode("//a[contains(text(),'clique aqui')]");

            if (elementoCliqueAqui != null)
            {
                var htmlElemento = elementoCliqueAqui.OuterHtml;

                var regexHref = new Regex(
                    @"href=""([^""]*amazon\.com[^""]*)""",
                    RegexOptions.IgnoreCase);

                var match = regexHref.Match(htmlElemento);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }

            return null;
        }
        catch
        {
            return null;
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

    private const string UrlBasePromobit = "https://www.promobit.com.br";
    private const string UrlRecentesPromobit = "https://www.promobit.com.br/promocoes/recentes/";
    private const string SeletorCartaoPromocao = "//a[contains(@class,'no-underline') and contains(@href,'/oferta/')]";

    private const string SeletorTitulo =
        ".//span[contains(@class,'whitespace-pre-wrap') and contains(@class,'line-clamp-2')]";

    private const string SeletorLoja = ".//span[contains(@class,'truncate') and contains(@class,'whitespace-nowrap')]";
    private static readonly string[] LojasPermitidas = ["Amazon", "Shopee"];
}