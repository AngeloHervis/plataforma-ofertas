using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using plataforma.ofertas._Base;
using plataforma.ofertas.Interfaces.Scrapers;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Scrapers;

public class MercadoLivreScraperService(HttpClient httpClient) : IMercadoLivreScraperService
{
    private static readonly string[] DefaultListaUrls =
    {
        "https://www.mercadolivre.com.br/social/tg20251019181553/lists/34e42115-2050-4a3d-ae1a-237fc26cf44b?matt_tool=42647359&forceInApp=true"
    };

    public async Task<CommandResult<List<Oferta>>> ObterInformacoesCompletasListaMercadoLivreAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var ofertas = new List<Oferta>();

            foreach (var url in DefaultListaUrls)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

                var response = await httpClient.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode) continue;

                var html = await response.Content.ReadAsStringAsync(cancellationToken);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var aTituloNodes = doc.DocumentNode
                    .SelectNodes("//div[contains(@class,'poly-card')]//a[contains(@class,'poly-component__title')]");
                if (aTituloNodes == null) continue;

                var vistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                var anchorsUnicos = aTituloNodes
                    .Select(a =>
                    {
                        var rawHref = a.GetAttributeValue("href", "")?.Trim() ?? "";
                        var tituloBruto = a.InnerText ?? "";
                        var key = BuildDedupKey(rawHref, tituloBruto);
                        return new { Node = a, RawHref = rawHref, TituloBruto = tituloBruto, Key = key };
                    })
                    .Where(x => !string.IsNullOrWhiteSpace(x.RawHref))
                    .Where(x => vistos.Add(x.Key))
                    .ToList();

                foreach (var a in anchorsUnicos)
                {
                    var card = a.Node.Ancestors("div")
                        .FirstOrDefault(n => n.GetClasses().Any(c => c.Contains("poly-card")));
                    if (card == null) continue;

                    var titulo = LimparTexto(a.TituloBruto);
                    var linkProduto = NormalizeMercadoLivreLink(a.RawHref);
                    if (string.IsNullOrWhiteSpace(linkProduto)) continue;

                    var info = await ObterInformacoesPaginaMercadoLivreAsync(linkProduto, cancellationToken);

                    var precoAnteriorValido = ValidarPrecoAnteriorValido(info.PrecoAnterior, info.PrecoAtual);
                    if (!precoAnteriorValido) info.PrecoAnterior = null;

                    ofertas.Add(new Oferta
                    {
                        Id = Guid.NewGuid(),
                        Fonte = "Mercado Livre",
                        Titulo = titulo,
                        PrecoAtual = info.PrecoAtual,
                        PrecoAnterior = info.PrecoAnterior,
                        DescontoPercentual = CalcularPercentual(info.PrecoAtual, info.PrecoAnterior),
                        Link = linkProduto,
                        ImagensUrl = info.ImagemUrl,
                        PublicadoEm = DateTime.UtcNow
                    });
                }
            }

            return CommandResult<List<Oferta>>.Success(ofertas);
        }
        catch (Exception ex)
        {
            return CommandResult<List<Oferta>>.InternalError(
                $"Erro ao raspar ofertas do Mercado Livre: {ex.Message}");
        }
    }

    public async Task<ProductInfo> ObterInformacoesCompletasMercadoLivreAsync(string linkDeal, CancellationToken ct)
    {
        return await ObterInformacoesPaginaMercadoLivreAsync(linkDeal, ct);
    }


    private static bool ValidarPrecoAnteriorValido(string precoAnterior, string precoAtual)
    {
        if (string.IsNullOrWhiteSpace(precoAnterior) || string.IsNullOrWhiteSpace(precoAtual))
            return false;

        var valorAnterior = ExtrairValorNumerico(precoAnterior);
        var valorAtual = ExtrairValorNumerico(precoAtual);

        if (!valorAnterior.HasValue || !valorAtual.HasValue)
            return false;

        return valorAnterior > valorAtual;
    }

    private async Task<ProductInfo> ObterInformacoesPaginaMercadoLivreAsync(
        string linkProduto,
        CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(linkProduto))
                return new ProductInfo();

            var request = new HttpRequestMessage(HttpMethod.Get, linkProduto);
            request.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

            var response = await httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var html = await response.Content.ReadAsStringAsync(ct);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var tituloNode = doc.DocumentNode.SelectSingleNode("//h1[contains(@class,'ui-pdp-title')]");
            var titulo = LimparTexto(tituloNode?.InnerText ?? "");

            var precoAtual = ExtrairPrecoAndes(doc.DocumentNode, true);
            var precoAnterior = ExtrairPrecoAndes(doc.DocumentNode, false);

            return new ProductInfo
            {
                Titulo = titulo,
                Link = linkProduto,
                PrecoAtual = precoAtual,
                PrecoAnterior = precoAnterior,
                ImagemUrl = ExtrairImagemPdp(doc),
            };
        }
        catch
        {
            return new ProductInfo();
        }
    }

    private static string ExtrairPrecoAndes(HtmlNode card, bool atual)
    {
        var root = card.SelectSingleNode(atual
            ? ".//div[contains(@class,'ui-pdp-price__second-line')]//span[contains(@class,'andes-money-amount')]"
            : ".//div[contains(@class,'poly-price__before')]//s"
              + "|.//s[contains(@class,'andes-money-amount')]");
    
        if (root == null) return null;
    
        var inteiro = root.SelectSingleNode(".//span[contains(@class,'andes-money-amount__fraction')]")?.InnerText?.Trim();
        var cents = root.SelectSingleNode(".//span[contains(@class,'andes-money-amount__cents')]")?.InnerText?.Trim();
    
        if (string.IsNullOrWhiteSpace(inteiro) || string.IsNullOrWhiteSpace(cents))
        {
            var aria = root.GetAttributeValue("aria-label", "");
            if (!string.IsNullOrWhiteSpace(aria))
                return ConverterAriaLabelParaPreco(aria);
    
            var metaPrice = root.SelectSingleNode(".//meta[@itemprop='price']")?.GetAttributeValue("content", "");
            if (!string.IsNullOrWhiteSpace(metaPrice) && metaPrice.Contains('.'))
            {
                var partes = metaPrice.Split('.');
                inteiro ??= partes[0];
                cents ??= partes.Length > 1 ? partes[1] : "00";
            }
        }
    
        if (!string.IsNullOrWhiteSpace(inteiro))
        {
            inteiro = inteiro.Replace(".", "");
            var centavos = cents ?? "00";
    
            if (decimal.TryParse($"{inteiro}.{centavos}", NumberStyles.Float, CultureInfo.InvariantCulture, out var valor))
            {
                return valor.ToString("C", new CultureInfo("pt-BR"));
            }
        }
    
        return null;
    }


    private static string ExtrairImagemPdp(HtmlDocument doc)
    {
        var imgZoom = doc.DocumentNode.SelectSingleNode("//img[contains(@class,'ui-pdp-image') and @data-zoom]");
        var zoom = imgZoom?.GetAttributeValue("data-zoom", "");
        if (!string.IsNullOrWhiteSpace(zoom)) return zoom;

        var img = doc.DocumentNode.SelectSingleNode("//img[contains(@class,'ui-pdp-image')]");
        var src = img?.GetAttributeValue("src", "");
        if (!string.IsNullOrWhiteSpace(src)) return src;

        var og = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
        var ogc = og?.GetAttributeValue("content", "");
        if (!string.IsNullOrWhiteSpace(ogc)) return ogc;

        return null;
    }

    private static string LimparTexto(string s)
        => string.IsNullOrWhiteSpace(s) ? "" : Regex.Replace(s, @"\s+", " ").Trim();

    private static string ConverterAriaLabelParaPreco(string aria)
    {
        var m = Regex.Match(aria, @"(\d[\d\.]*)\s*reais(?:\s*com\s*(\d{1,2})\s*centavos)?",
            RegexOptions.IgnoreCase);
        if (!m.Success) return null;

        var inteiro = m.Groups[1].Value.Replace(".", ""); // Remove pontos (separadores de milhares)
        var cents = m.Groups[2].Success ? m.Groups[2].Value.PadLeft(2, '0') : "00";

        if (decimal.TryParse($"{inteiro}.{cents}", NumberStyles.Float, CultureInfo.InvariantCulture, out var valor))
        {
            return valor.ToString("C", new CultureInfo("pt-BR"));
        }

        return null;
    }

    private static int? CalcularPercentual(string precoAtual, string precoAnterior)
    {
        if (string.IsNullOrWhiteSpace(precoAnterior) || string.IsNullOrWhiteSpace(precoAtual)) return null;

        var valorAtual = ExtrairValorNumerico(precoAtual);
        var valorAnterior = ExtrairValorNumerico(precoAnterior);

        if (!valorAtual.HasValue || !valorAnterior.HasValue || valorAnterior <= 0) return null;

        var desconto = (int)(((valorAnterior.Value - valorAtual.Value) * 100) / valorAnterior.Value);
        return desconto;
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

    private static string BuildDedupKey(string href, string tituloFallback)
    {
        // 1) Se extrair o ID (ex.: MLB123456789), use-o — independe de params.
        var id = ExtractMercadoLivreId(href);
        if (!string.IsNullOrEmpty(id)) return $"ID:{id}";

        // 2) Senão, dedup por URL normalizada (sem utm/matt/from/sid/attributes etc.)
        var norm = NormalizeMercadoLivreLink(href);
        if (!string.IsNullOrEmpty(norm)) return $"URL:{norm}";

        // 3) Fallback: título "slugado" (melhor que nada)
        return "TTL:" + Slugify(LimparTexto(tituloFallback));
    }

    private static string ExtractMercadoLivreId(string href)
    {
        // cobre MLB-123456789, MLB123456789, etc.
        var m = Regex.Match(href ?? "", @"\b(ML[A-Z]{1}\-?\d{6,})\b", RegexOptions.IgnoreCase);
        if (m.Success) return m.Groups[1].Value.Replace("-", "").ToUpperInvariant();
        return null;
    }

    private static string NormalizeMercadoLivreLink(string href)
    {
        if (string.IsNullOrWhiteSpace(href)) return null;

        if (!Uri.TryCreate(href, UriKind.Absolute, out var uri))
        {
            Uri.TryCreate("https://www.mercadolivre.com.br", UriKind.Absolute, out var baseUri);
            Uri.TryCreate(baseUri, href, out uri);
        }

        if (uri == null) return null;

        var blacklist = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "utm_source", "utm_medium", "utm_campaign", "utm_term", "utm_content",
            "matt_tool", "matt_word", "matt_source", "matt_campaign", "matt_device",
            "matt_product_id", "matt_campaign_id", "c_id", "from", "sid", "attributes", "forceInApp"
        };

        var q = ParseQuery(uri.Query);
        var kept = q
            .Where(kv => !blacklist.Contains(kv.Key))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value ?? "")}")
            .ToArray();

        var cleanQuery = kept.Length > 0 ? "?" + string.Join("&", kept) : "";
        return $"{uri.Scheme}://{uri.Host.ToLowerInvariant()}{uri.AbsolutePath.TrimEnd('/')}{cleanQuery}";
    }

    private static IEnumerable<KeyValuePair<string, string>> ParseQuery(string query)
    {
        if (string.IsNullOrEmpty(query)) yield break;
        foreach (var kv in query.TrimStart('?')
                     .Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = kv.Split('=', 2);
            var k = Uri.UnescapeDataString(parts[0]);
            var v = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]) : "";
            yield return new KeyValuePair<string, string>(k, v);
        }
    }

    private static string Slugify(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "";
        var t = s.ToLowerInvariant();
        t = Regex.Replace(t, @"[^\p{L}\p{Nd}]+", "-");
        t = Regex.Replace(t, @"-+", "-").Trim('-');
        return t;
    }
}