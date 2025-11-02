using Microsoft.AspNetCore.Mvc;
using plataforma.ofertas.Dto.Constantes;
using plataforma.ofertas.Dto.Templates;
using plataforma.ofertas.Extensions;
using plataforma.ofertas.Interfaces.Templates;

namespace plataforma.ofertas.Controllers;

/// <summary>
/// API de Templates
/// </summary>
[ApiController]
[Route("api/templates")]
public class TemplatesController : ControllerBase
{
    /// <summary>
    /// Retorna todos os Templates ativos
    /// </summary>
    /// <param name="service">Serviço de consulta de Templates</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Sucesso</response>
    /// <response code="403">Usuário não autorizado</response>
    /// <response code="500">Erro interno</response>
    [HttpGet]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(List<TemplateDetalhesDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTemplatesAtivos(
        [FromServices] IConsultaTemplatesService service,
        CancellationToken cancellationToken)
    {
        return await service.ConsultarAsync(cancellationToken).ToResponseResultAsync();
    }

    [HttpDelete("{id:guid}")]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeletarTemplate(
        [FromServices] IDeletarTemplateService service,
        Guid id,
        CancellationToken cancellationToken)
    {
        return await service.DeletarAsync(id, cancellationToken).ToResponseResultAsync();
    }

    [HttpPost]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CriarTemplate(
        [FromServices] ICriarTemplateService service,
        [FromBody] CriarTemplateDto dto,
        CancellationToken cancellationToken)
    {
        return await service.CriarAsync(dto, cancellationToken).ToResponseResultAsync();
    }
}