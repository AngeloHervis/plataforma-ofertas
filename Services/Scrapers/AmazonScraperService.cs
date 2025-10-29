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
            var linkAmazon = AplicarTagAfiliado(linkDeal);

            if (string.IsNullOrEmpty(linkAmazon))
                return new Oferta();

            var oferta = await scraperApiService.ObterOfertaAmazonAsync(linkAmazon, fonte, ct);
            return oferta ?? new Oferta();
        }
        catch
        {
            return new Oferta();
        }
    }

    private static string AplicarTagAfiliado(string url)
    {
        if (string.IsNullOrEmpty(url) || !url.Contains("amazon."))
            return url;

        var tag = "beloto-20";
        var match = Regex.Match(url, @"(https://www\.amazon\.com\.br/dp/[A-Za-z0-9]+)");
        if (match.Success)
            url = match.Groups[1].Value;


        return $"{url}?tag={tag}";
    }
}