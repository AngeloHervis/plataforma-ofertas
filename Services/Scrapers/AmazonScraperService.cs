using System.Text.RegularExpressions;
using System.Globalization;
using HtmlAgilityPack;
using plataforma.ofertas.Interfaces.Scrapers;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Scrapers;

public class AmazonScraperService(HttpClient httpClient) : IAmazonScraperService
{
    public async Task<ProductInfo> ObterInformacoesCompletasDaAmazonAsync(string linkDeal, string fonte,
        CancellationToken ct)
    {
        try
        {
            var linkAmazon = AplicarTagAfiliado(linkDeal, fonte);

            if (string.IsNullOrEmpty(linkAmazon))
                return new ProductInfo();

            var request = new HttpRequestMessage(HttpMethod.Get, linkAmazon);
            request.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            var response = await httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var htmlAmazon = await response.Content.ReadAsStringAsync(ct);
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlAmazon);
            var precoAnterior = ExtrairPrecoAnteriorDaAmazon(doc);
            var precoAtual = ExtrairPrecoDaAmazon(doc);

            var precoAnteriorValido = ValidarPrecoAnteriorValido(precoAnterior, precoAtual);
            if (!precoAnteriorValido) precoAnterior = null;

            return new ProductInfo
            {
                Titulo = ExtrairTituloDaAmazon(doc),
                Link = linkAmazon,
                ImagemUrl = ExtrairImagemDaAmazon(doc),
                PrecoAtual = precoAtual,
                PrecoAnterior = precoAnterior
            };
        }
        catch
        {
            return new ProductInfo();
        }
    }

    private bool ValidarPrecoAnteriorValido(string precoAnterior, string precoAtual)
    {
        if (string.IsNullOrWhiteSpace(precoAnterior) || string.IsNullOrWhiteSpace(precoAtual))
            return false;

        var valorAnterior = ExtrairValorNumerico(precoAnterior);
        var valorAtual = ExtrairValorNumerico(precoAtual);

        if (!valorAnterior.HasValue || !valorAtual.HasValue)
            return false;

        return valorAnterior > valorAtual;
    }

    private static decimal? ExtrairValorNumerico(string preco)
    {
        if (string.IsNullOrWhiteSpace(preco)) return null;

        var precoLimpo = preco.Replace("R$", "").Replace(" ", "").Trim();

        if (precoLimpo.Contains(','))
        {
            var partes = precoLimpo.Split(',');
            if (partes.Length == 2 && partes[1].Length == 2)
            {
                var parteInteira = partes[0].Replace(".", "");
                var centavos = partes[1];

                if (decimal.TryParse($"{parteInteira}.{centavos}", NumberStyles.Float, CultureInfo.InvariantCulture,
                        out var valor))
                    return valor;
            }
        }

        return null;
    }

    private static string AplicarTagAfiliado(string url, string fonte)
    {
        if (string.IsNullOrEmpty(url) || !url.Contains("amazon."))
            return url;

        var tag = "beloto-20";
        var urlLimpa = url;

        if (fonte == "Promobit")
        {
            var match = Regex.Match(url, @"(https://www\.amazon\.com\.br/[^/]+/dp/[^/?]+)");
            if (match.Success)
            {
                urlLimpa = match.Groups[1].Value;
            }
        }

        var urlSemTag = Regex.Replace(urlLimpa, @"[?&]tag=[^&]*", "");

        var separador = urlSemTag.Contains('?') ? "&" : "?";
        return $"{urlSemTag}{separador}tag={tag}";
    }

    private static string ExtrairTituloDaAmazon(HtmlDocument doc)
    {
        var seletores = new[]
        {
            "//span[@id='productTitle']",
            "//h1[@id='title']//span[@id='productTitle']",
            "//h1[contains(@class,'product-title')]"
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

    private static string ExtrairImagemDaAmazon(HtmlDocument doc)
    {
        var seletores = new[]
        {
            "//img[@id='landingImage']",
            "//div[@id='imgTagWrapperId']//img",
            "//img[contains(@class,'a-dynamic-image')]"
        };

        foreach (var seletor in seletores)
        {
            var elemento = doc.DocumentNode.SelectSingleNode(seletor);
            if (elemento != null)
            {
                var src = elemento.GetAttributeValue("src", "");
                if (!string.IsNullOrWhiteSpace(src))
                    return src;
            }
        }

        return string.Empty;
    }

    private static string ExtrairPrecoDaAmazon(HtmlDocument doc)
    {
        // Seletores específicos para preço atual em promoção
        var seletoresPrecoAtual = new[]
        {
            "//span[contains(@class,'priceToPay')]//span[@class='a-offscreen']",
            "//span[contains(@class,'priceToPay')]//span[@aria-hidden='true']",
            "//span[@class='aok-offscreen'][contains(text(),'R$')]",
            "//span[contains(@class,'a-price-whole')]",
            "//span[@class='a-price a-text-price a-size-medium a-color-base']//span[@class='a-offscreen']",
            "//span[contains(@class,'a-price')]//span[@class='a-offscreen']"
        };

        foreach (var seletor in seletoresPrecoAtual)
        {
            var elemento = doc.DocumentNode.SelectSingleNode(seletor);
            if (elemento == null) continue;

            var precoTexto = elemento.InnerText?.Trim();
            var preco = ProcessarPrecoAmazon(precoTexto);
            if (!string.IsNullOrWhiteSpace(preco))
                return preco;
        }

        try
        {
            var simbolo = doc.DocumentNode.SelectSingleNode("//span[contains(@class,'a-price-symbol')]")?.InnerText
                ?.Trim();
            var inteiro = doc.DocumentNode.SelectSingleNode("//span[contains(@class,'a-price-whole')]")?.InnerText
                ?.Trim();
            var fracao = doc.DocumentNode.SelectSingleNode("//span[contains(@class,'a-price-fraction')]")?.InnerText
                ?.Trim();

            if (!string.IsNullOrEmpty(inteiro) && !string.IsNullOrEmpty(fracao))
            {
                var precoTexto = $"{simbolo ?? ""}{inteiro},{fracao}";
                return ProcessarPrecoAmazon(precoTexto);
            }
        }
        catch
        {
            /* Ignora erros na extração de preço */
        }

        return null;
    }

    private static string ExtrairPrecoAnteriorDaAmazon(HtmlDocument doc)
    {
        var seletoresPrecoAnterior = new[]
        {
            "//span[contains(@class,'basisPrice')]//span[@class='a-offscreen']",
            "//span[contains(@class,'a-text-price') and @data-a-strike='true']//span[@class='a-offscreen']",
            "//span[contains(@class,'a-text-price') and contains(@data-a-strike,'true')]//span[@aria-hidden='true']",
            "//span[contains(text(),'De:')]//following-sibling::span//span[@class='a-offscreen']",
            "//span[contains(@class,'a-price-was')]//span[@class='a-offscreen']"
        };

        foreach (var seletor in seletoresPrecoAnterior)
        {
            var elemento = doc.DocumentNode.SelectSingleNode(seletor);
            if (elemento == null) continue;

            var precoTexto = elemento.InnerText?.Trim();
            var preco = ProcessarPrecoAmazon(precoTexto);
            if (!string.IsNullOrWhiteSpace(preco))
                return preco;
        }

        return null;
    }

    private static string ProcessarPrecoAmazon(string precoTexto)
    {
        if (string.IsNullOrWhiteSpace(precoTexto))
            return null;

        var precoLimpo = precoTexto
            .Replace("R$", "")
            .Replace("&nbsp;", "")
            .Replace(" ", "")
            .Trim();

        if (precoLimpo.Contains(','))
        {
            var partes = precoLimpo.Split(',');
            if (partes.Length == 2 && partes[1].Length == 2)
            {
                var parteInteira = partes[0];
                var centavos = partes[1];

                // Remove pontos da parte inteira (separadores de milhares)
                parteInteira = parteInteira.Replace(".", "");

                // Reconstrói o valor no formato correto
                if (decimal.TryParse($"{parteInteira}.{centavos}", System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out var valor))
                {
                    return valor.ToString("C", new CultureInfo("pt-BR"));
                }
            }
        }

        // Formato sem vírgula: tenta interpretar como centavos
        if (!precoLimpo.Contains(',') && !precoLimpo.Contains('.'))
        {
            if (decimal.TryParse(precoLimpo, out var valorCentavos))
            {
                var valorReal = valorCentavos / 100;
                return valorReal.ToString("C", new CultureInfo("pt-BR"));
            }
        }

        return null;
    }
}