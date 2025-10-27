using System.Text.RegularExpressions;
using plataforma.ofertas.Interfaces;
using plataforma.ofertas.Interfaces.Scrapers;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Scrapers;

public class AmazonScraperService(IScraperApiService scraperApiService) : IAmazonScraperService
{
    public async Task<Oferta> ObterInformacoesCompletasDaAmazonAsync(string linkDeal, string fonte,
        CancellationToken ct)
    {
        try
        {
            var linkAmazon = AplicarTagAfiliado(linkDeal, fonte);

            if (string.IsNullOrEmpty(linkAmazon))
                return new Oferta();
            
            var oferta = await scraperApiService.ObterOfertaAmazonAsync(linkAmazon, ct);
            return oferta ?? new Oferta();
        }
        catch
        {
            return new Oferta();
        }
    }

    private static string AplicarTagAfiliado(string url, string fonte)
    {
        if (string.IsNullOrEmpty(url) || !url.Contains("amazon."))
            return url;

        var tag = "beloto-20";
        var urlLimpa = url;

        var match = Regex.Match(urlLimpa, @"(https://www\.amazon\.com\.br/dp/[^/?]+)");
        if (match.Success)
        {
            urlLimpa = match.Groups[1].Value;
        }

        return $"{urlLimpa}?tag={tag}";
    }
}