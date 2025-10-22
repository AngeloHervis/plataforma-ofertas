using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using plataforma.ofertas._Base;
using plataforma.ofertas.Interfaces.Scrapers;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Scrapers;

public class ShopeeScraperService(HttpClient httpClient) : IShopeeScraperService
{
    private const string SeletorBotaoLoja = "//a[contains(@class,'store-link-button')]";
    private static readonly Regex RegexLimpezaPreco = new(@"[^\d,\.]", RegexOptions.Compiled);

    public async Task<ProductInfo> ObterInformacoesCompletasDaShopeeAsync(string linkDeal, CancellationToken ct)
    {
        try
        {
            var linkShopee = await ObterLinkRealDaShopeeAsync(linkDeal, ct);
            if (string.IsNullOrEmpty(linkShopee))
                return new ProductInfo();

            var htmlShopee = await httpClient.GetStringAsync(linkShopee, ct);
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlShopee);

            RemoverVideos(doc);

            return new ProductInfo
            {
                Titulo = ExtrairTituloDaShopee(doc),
                Link = linkShopee,
                ImagemUrl = ExtrairImagemDaShopee(doc),
                PrecoAtual = ExtrairPrecoAtualDaShopee(doc),
                PrecoAnterior = ExtrairPrecoAnteriorDaShopee(doc)
            };
        }
        catch
        {
            return new ProductInfo();
        }
    }

    private static void RemoverVideos(HtmlDocument doc)
    {
        var videosNodes = doc.DocumentNode.SelectNodes("//video");
        if (videosNodes != null)
        {
            foreach (var videoNode in videosNodes)
            {
                videoNode.Remove();
            }
        }
    }

    private async Task<string> ObterLinkRealDaShopeeAsync(string linkDeal, CancellationToken ct)
    {
        try
        {
            var html = await httpClient.GetStringAsync(linkDeal, ct);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var botao = doc.DocumentNode.SelectSingleNode(SeletorBotaoLoja);
            if (botao == null) return null;

            var urlRedirect = botao.GetAttributeValue("href", null);
            if (string.IsNullOrEmpty(urlRedirect)) return null;

            using var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = true;
            handler.MaxAutomaticRedirections = 10;
            
            using var client = new HttpClient(handler);
            var response = await client.GetAsync(urlRedirect, HttpCompletionOption.ResponseHeadersRead, ct);
            return response.RequestMessage?.RequestUri?.ToString();
        }
        catch
        {
            return null;
        }
    }

    private static string ExtrairTituloDaShopee(HtmlDocument doc)
    {
        var seletores = new[]
        {
            "//div[contains(@class,'product-briefing')]//div[contains(@class,'_44qnta')]",
            "//div[contains(@class,'attM6y')]//span",
            "//div[contains(@class,'product-title')]"
        };

        foreach (var seletor in seletores)
        {
            var elemento = doc.DocumentNode.SelectSingleNode(seletor);
            if (elemento != null)
            {
                var texto = elemento.InnerText?.Trim();
                if (!string.IsNullOrWhiteSpace(texto))
                    return texto;
            }
        }

        return string.Empty;
    }

    private static string ExtrairImagemDaShopee(HtmlDocument doc)
    {
        var imagem = doc.DocumentNode.SelectSingleNode(".//img[contains(@src, 'susercontent.com/file/')]");
        if (imagem != null)
        {
            var src = imagem.GetAttributeValue("src", "");
            if (!string.IsNullOrWhiteSpace(src))
            {
                return src.Replace("_tn", "");
            }
        }

        return string.Empty;
    }

    private string ExtrairPrecoAtualDaShopee(HtmlDocument doc)
    {
        var seletores = new[]
        {
            "//div[contains(@class,'pqTWkA')]",
            "//div[contains(@class,'_3n5NQx')]"
        };

        foreach (var seletor in seletores)
        {
            var elemento = doc.DocumentNode.SelectSingleNode(seletor);
            if (elemento != null)
            {
                var precoTexto = elemento.InnerText?.Trim();
                var preco = ProcessarPreco(precoTexto);
                if (!string.IsNullOrWhiteSpace(preco))
                    return preco;
            }
        }

        return null;
    }

    private string? ExtrairPrecoAnteriorDaShopee(HtmlDocument doc)
    {
        var seletores = new[]
        {
            "//div[contains(@class,'_3_ISdg')]//span[contains(@class,'_1k1Vcm')]",
            "//div[contains(@class,'_3n5NQx')]/following-sibling::div[contains(@class,'rXF0YZ')]"
        };

        foreach (var seletor in seletores)
        {
            var elemento = doc.DocumentNode.SelectSingleNode(seletor);
            if (elemento != null)
            {
                var precoTexto = elemento.InnerText?.Trim();
                var preco = ProcessarPreco(precoTexto);
                if (!string.IsNullOrWhiteSpace(preco))
                    return preco;
            }
        }

        return null;
    }

    private static string ProcessarPreco(string precoTexto)
    {
        if (string.IsNullOrWhiteSpace(precoTexto))
            return null;

        var precoLimpo = RegexLimpezaPreco.Replace(precoTexto, "")
            .Replace("R$", "")
            .Replace(" ", "")
            .Replace(",", ".")
            .Trim();

        return precoLimpo;
    }
}