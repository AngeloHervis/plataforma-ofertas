using System.Globalization;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Extensions;

public static class HelpersExtensions
{
    public static int? CalcularPercentual(string precoAtual, string precoAnterior)
    {
        if (string.IsNullOrEmpty(precoAnterior) || string.IsNullOrEmpty(precoAtual))
            return null;

        var valorAtual = ExtrairValorDecimal(precoAtual);
        var valorAnterior = ExtrairValorDecimal(precoAnterior);

        if (!valorAtual.HasValue || !valorAnterior.HasValue || valorAnterior <= 0)
            return null;

        var desconto = (int)((valorAnterior.Value - valorAtual.Value) * 100 / valorAnterior.Value);
        return desconto;
    }
    
    public static string PadronizarPreco(this string preco)
    {
        if (string.IsNullOrWhiteSpace(preco))
            return string.Empty;

        var valor = preco.Replace("R$", "").Replace(" ", "").Trim();
        return $"R$ {valor}";
    }

    private static decimal? ExtrairValorDecimal(string preco)
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

                if (decimal.TryParse($"{parteInteira}.{centavos}", NumberStyles.Float,
                        CultureInfo.InvariantCulture, out var valor))
                    return valor;
            }
        }

        return null;
    }
    
    public static List<string> ObterListaImagens(string imagensUrl, Oferta oferta)
    {
        if (string.IsNullOrWhiteSpace(imagensUrl))
            return [];

        var listaImagens = ConverterStringParaLista(oferta.ImagensUrl);
        return listaImagens.OrderBy(img => img == oferta.ImagemUrlPrincipal ? 0 : 1).ToList();
    }

    public static string ConverterListaParaString(List<string> urls)
        => string.Join(",", urls);

    public static List<string> ConverterStringParaLista(string urls)
        => urls.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();

    private static readonly char[] separator = [','];
}