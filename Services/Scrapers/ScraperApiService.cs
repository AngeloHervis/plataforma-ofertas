using System.Text.RegularExpressions;
using plataforma.ofertas.Extensions;
using plataforma.ofertas.Interfaces;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Scrapers;

public class ScraperApiService : IScraperApiService
{
    public async Task<Oferta> ObterOfertaAmazonAsync(string url, string fonte, CancellationToken ct)
    {
        try
        {
            var urlScraperApi =
                $"https://api.scraperapi.com/?api_key=ac361cfaccb03eb145cbce493dc0ecba&url={Uri.EscapeDataString(url)}&output_format=markdown&device_type=desktop&country_code=br";
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(urlScraperApi, ct);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error in API Call");

            var responseBody = await response.Content.ReadAsStringAsync(ct);
            var oferta = ExtrairOfertaDeMarkdown(responseBody, url, fonte);
            return oferta;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }

    private Oferta ExtrairOfertaDeMarkdown(string responseBody, string url, string fonte)
    {
        try
        {
            var precosMatches = Regex.Matches(responseBody, @"R\$\s*(\d{1,3}(?:\.\d{3})*(?:,\d{2})?)");
            var todosPrecos = precosMatches.Select(m => m.Value).ToList();

            var precoAnteriorMatch = Regex.Match(responseBody, @"De:\s*(R\$\s*\d{1,3}(?:\.\d{3})*(?:,\d{2})?)");
            var precoAnterior = precoAnteriorMatch.Success ? precoAnteriorMatch.Groups[1].Value : string.Empty;

            var precoAtual = string.Empty;
            if (!string.IsNullOrEmpty(precoAnterior))
            {
                var posicaoPrecoAnterior = todosPrecos.IndexOf(precoAnterior);
                if (posicaoPrecoAnterior > 0)
                    precoAtual = todosPrecos[posicaoPrecoAnterior - 1];
            }

            if (string.IsNullOrEmpty(precoAtual) && todosPrecos.Count > 0)
                precoAtual = todosPrecos[0];

            var (listaUrlsAntesDePrecoAnterior, titulo) = ExtrairImagensETitulo(responseBody);
            var listaUrlsCorrigidas = listaUrlsAntesDePrecoAnterior
                .Where((urlImagem, index) => !(index == 0 && urlImagem.EndsWith(".png")))
                .Select(urlImagem => urlImagem.EndsWith("..jpg") ? urlImagem.Replace("..jpg", ".jpg") : urlImagem)
                .ToList();

            if (string.IsNullOrEmpty(fonte))
                fonte = ExtrairFonteDaUrl(url);

            var descontoPercentual = HelpersExtensions.CalcularPercentual(precoAtual, precoAnterior);

            return new Oferta
            {
                Id = Guid.NewGuid(),
                Fonte = fonte,
                Titulo = titulo,
                PrecoAtual = precoAtual,
                PrecoAnterior = precoAnterior,
                DescontoPercentual = descontoPercentual,
                Link = url,
                ImagensUrl = HelpersExtensions.ConverterListaParaString(listaUrlsAntesDePrecoAnterior),
                ImagemUrlPrincipal = listaUrlsCorrigidas.FirstOrDefault() ?? string.Empty,
                PublicadoEm = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao extrair dados do markdown: {ex.Message}");
            return null;
        }
    }

    private static (List<string>, string) ExtrairImagensETitulo(string responseBody)
    {
        var listaValoresOuImagens = new List<string>();
        var regexValor = new Regex(@"R\$\s*\d{1,3}(?:\.\d{3})*(?:,\d{2})?");
        var regexImagem = new Regex(@"!\[.*?\]\((https://m\.media-amazon\.com/images/I/[^\s)]+)\)");
        var regexTitulo = new Regex(@"!\[(.*?)\]");

        var linhas = responseBody.Split('\n');
        var titulo = "Produto";
        var linhaAnterior = string.Empty;

        for (var i = 0; i < linhas.Length; i++)
        {
            var linha = linhas[i];

            if (linha.Contains("Para ver este vídeo faça o download") && !string.IsNullOrEmpty(linhaAnterior))
            {
                var matchTitulo = regexTitulo.Match(linhaAnterior);
                if (matchTitulo.Success)
                {
                    titulo = matchTitulo.Groups[0].Value.Trim();
                }
            }

            var matchValor = regexValor.Match(linha);
            if (matchValor.Success)
                listaValoresOuImagens.Add(matchValor.Value);

            var matchImagem = regexImagem.Match(linha);
            if (matchImagem.Success)
                listaValoresOuImagens.Add(matchImagem.Groups[1].Value);

            linhaAnterior = linha;
        }

        var listaUrlsAntesDePrecoAnterior = new List<string>();
        var precoAnteriorMatch = Regex.Match(responseBody, @"De:\s*(R\$\s*\d{1,3}(?:\.\d{3})*(?:,\d{2})?)");
        var precoAnterior = precoAnteriorMatch.Success
            ? precoAnteriorMatch.Groups[1].Value.Replace("De:", "").Trim()
            : string.Empty;

        for (var i = 0; i < listaValoresOuImagens.Count; i++)
        {
            if (precoAnterior == listaValoresOuImagens[i])
            {
                for (var j = 0; j < i; j++)
                {
                    if (listaValoresOuImagens[j].StartsWith("https://"))
                        listaUrlsAntesDePrecoAnterior.Add(listaValoresOuImagens[j]);
                }
            }
        }

        var imagensLimpas = listaUrlsAntesDePrecoAnterior
            .Select(url => Regex.Replace(url, @"_AC_[^\.]+(?=\.)|\.\.", "")).Distinct()
            .ToList();

        return (imagensLimpas, LimparTitulo(titulo));
    }

    private string ExtrairTituloDoUrl(string responseBody)
    {
        try
        {
            var match = Regex.Match(responseBody, @"#\s*Resumo do produto:\s*(.+)");
            if (match.Success)
            {
                var titulo = match.Groups[1].Value.Trim();
                return titulo;
            }

            return "Produto";
        }
        catch
        {
            return "Produto";
        }
    }

    private static string LimparTitulo(string titulo)
    {
        try
        {
            var tituloLimpo = Regex.Replace(titulo, @"[\[\]!]", "").Trim();
            return tituloLimpo;
        }
        catch
        {
            return titulo;
        }
    }

    private static string ExtrairFonteDaUrl(string url)
    {
        try
        {
            if (url.Contains("amazon"))
                return "Amazon";
            if (url.Contains("mercadolivre"))
                return "Mercado Livre";

            return url.Contains("shopee") ? "Shopee" : url;
        }
        catch
        {
            return "Desconhecido";
        }
    }
}