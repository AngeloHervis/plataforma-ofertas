using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using plataforma.ofertas.Interfaces.Scrapers;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Scrapers;

public class ShopeeScraperService(HttpClient httpClient) : IShopeeScraperService
{
    public async Task<ProductInfo> ObterInformacoesCompletasDaShopeeAsync(string linkDeal, CancellationToken ct)
    {
        try
        {
            var linkAfiliado = await GerarLinkAfiliadoAsync(linkDeal, ct);

            var htmlShopee = await httpClient.GetStringAsync(linkDeal, ct);
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlShopee);

            RemoverVideos(doc);

            return new ProductInfo
            {
                Titulo = ExtrairTituloDaShopee(doc, linkDeal),
                Link = linkAfiliado,
                ImagemUrl = ExtrairImagemDaShopee(doc),
                PrecoAtual = "Shopee não divulga preços em sua página pública",
                PrecoAnterior = "Shopee não divulga preços em sua página pública"
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

    private async Task<string> GerarLinkAfiliadoAsync(string originUrl, CancellationToken ct)
    {
        var baseUrl = "https://open-api.affiliate.shopee.com.br/graphql";
        var partnerId = "18344990407";
        var partnerKey = "MZ77GUS36MRZ6YYYYOVV5SMC6QZQ7O7W";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var body = new
        {
            query = "mutation { generateShortLink(input:{ originUrl:\"" + originUrl +
                    "\", subIds:[\"s1\",\"s2\",\"s3\",\"s4\",\"s5\"] }) { shortLink } }"
        };

        var json = JsonSerializer.Serialize(body);
        var signature = GerarAssinatura(partnerId, partnerKey, timestamp, json);

        var authHeader = $"SHA256 Credential={partnerId}, Timestamp={timestamp}, Signature={signature}";
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Authorization", authHeader);

        var response =
            await httpClient.PostAsync(baseUrl, new StringContent(json, Encoding.UTF8, "application/json"), ct);

        if (!response.IsSuccessStatusCode)
            return originUrl;

        var result = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(result);
        var shortLink = doc.RootElement
            .GetProperty("data")
            .GetProperty("generateShortLink")
            .GetProperty("shortLink")
            .GetString();

        return shortLink ?? originUrl;
    }

    private static string GerarAssinatura(string partnerId, string partnerKey, string timestamp, string payload)
    {
        var factor = $"{partnerId}{timestamp}{payload}{partnerKey}";
        var bytes = Encoding.UTF8.GetBytes(factor);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLower();
    }

    private static string ExtrairTituloDaShopee(HtmlDocument doc, string linkShopee)
    {
        if (string.IsNullOrWhiteSpace(linkShopee))
            return string.Empty;

        try
        {
            var match = Regex.Match(linkShopee, @"shopee\.com\.br\/(?<titulo>.+)-i\.\d+\.\d+");
            if (!match.Success)
                return string.Empty;

            var tituloUrl = match.Groups["titulo"].Value;

            tituloUrl = Uri.UnescapeDataString(tituloUrl);

            tituloUrl = tituloUrl.Replace("-", " ");

            var textoFinal = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tituloUrl.ToLower());

            return textoFinal.Trim();
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string ExtrairImagemDaShopee(HtmlDocument doc)
    {
        var html = doc.DocumentNode.InnerHtml;

        var matches = Regex.Matches(html, "\"images\"\\s*:\\s*\\[(?<imgs>[^\\]]+)\\]");

        if (matches.Count >= 2)
        {
            var segundo = matches[1].Groups["imgs"].Value;

            var innerMatch = Regex.Match(segundo, "\"(?<img>br-[^\"]+)\"");
            if (innerMatch.Success)
            {
                var id = innerMatch.Groups["img"].Value;
                return $"https://down-br.img.susercontent.com/file/{id}";
            }
        }

        if (matches.Count == 1)
        {
            var primeiro = matches[0].Groups["imgs"].Value;
            var innerMatch = Regex.Match(primeiro, "\"(?<img>br-[^\"]+)\"");
            if (innerMatch.Success)
            {
                var id = innerMatch.Groups["img"].Value;
                return $"https://down-br.img.susercontent.com/file/{id}";
            }
        }

        var img = doc.DocumentNode
            .SelectSingleNode("//img[contains(@src,'susercontent.com/file/')]");
        var url = img?.GetAttributeValue("src", null);

        if (!string.IsNullOrWhiteSpace(url))
            return url.Split('?')[0];

        return string.Empty;
    }
}