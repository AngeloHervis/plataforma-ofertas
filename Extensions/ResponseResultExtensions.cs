using Microsoft.AspNetCore.Mvc;
using plataforma.ofertas._Base;

namespace plataforma.ofertas.Extensions;

/// <summary>
/// Obter resposta do resultado
/// </summary>
public static class ResponseResultExtensions
{
    /// <summary>
    /// Converter CommandResult em ActionResult assincronamente
    /// </summary>
    /// <param name="result">Informe o CommandResult</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Retorna ActionResult</returns>
    public static async Task<IActionResult> ToResponseResultAsync<T>(this Task<CommandResult<T>>  result)
    {
        var resultado = await result;
        return await Task.FromResult(new ObjectResult(resultado.IsError ? resultado.Errors : resultado.Data)
        {
            StatusCode = resultado.StatusCode
        });
    }
}