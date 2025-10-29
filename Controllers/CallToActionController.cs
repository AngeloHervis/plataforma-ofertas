using Microsoft.AspNetCore.Mvc;
using plataforma.ofertas.Dto.Constantes;
using plataforma.ofertas.Dto.CTAs;
using plataforma.ofertas.Extensions;
using plataforma.ofertas.Interfaces.CTAs;

namespace plataforma.ofertas.Controllers;

/// <summary>
/// API de Call To Action
/// </summary>
[ApiController]
[Route("api/ctas")]
public class CallToActionController : ControllerBase
{
    /// <summary>
    /// Retorna todas as Call To Actions ativas
    /// </summary>
    /// <param name="service">Serviço de consulta de CTAs</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Sucesso</response>
    /// <response code="403">Usuário não autorizado</response>
    /// <response code="500">Erro interno</response>
    [HttpGet]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(List<CtaDetalhesDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterCtasAtivas(
        [FromServices] IConsultaCtasService service,
        CancellationToken cancellationToken)
    {
        return await service.ConsultarAsync(cancellationToken).ToResponseResultAsync();
    }

    [HttpDelete("{id:guid}")]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeletarCta(
        [FromServices] IDeletarCtaService service,
        Guid id,
        CancellationToken cancellationToken)
    {
        return await service.DeletarAsync(id, cancellationToken).ToResponseResultAsync();
    }

    [HttpPost]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CriarCta(
        [FromServices] ICriarCtaService service,
        [FromBody] CriarCtaDto dto,
        CancellationToken cancellationToken)
    {        return await service.CriarAsync(dto, cancellationToken).ToResponseResultAsync();   

}

}