using Microsoft.AspNetCore.Mvc;
using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Constantes;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Interfaces.Scrapers;

namespace plataforma.ofertas.Controllers;

/// <summary>
/// API de Ofertas — Integração com scrapers (Pelando, Promobit, Amazon, Shopee, Mercado Livre)
/// </summary>
[ApiController]
[Route("api/ofertas")]
public class OfertasController : ControllerBase
{
    /// <summary>
    /// Retorna todas as ofertas mais recentes (Pelando, Promobit, Amazon, Shopee, Mercado Livre)
    /// </summary>
    /// <param name="service">Serviço de consulta de ofertas recentes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Sucesso</response>
    /// <response code="403">Usuário não autorizado</response>
    /// <response code="500">Erro interno</response>
    [HttpGet("recentes")]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(List<OfertaResumoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterOfertasRecentes(
        [FromServices] IConsultaOfertasDoBancoService service,
        CancellationToken cancellationToken)
    {
        return await service.ConsultarAsync(cancellationToken).ToResponseResultAsync();
    }
    
    /// <summary>
    /// Retorna os detalhes de uma oferta específica
    /// </summary>
    /// <param name="id">Identificador único da oferta</param>
    /// <param name="service">Serviço de consulta de detalhes da oferta</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <response code="200">Sucesso</response>
    /// <response code="403">Usuário não autorizado</response>
    /// <response code="404">Oferta não encontrada</response>
    /// <response code="500">Erro interno</response>
    [HttpGet("{id:guid}")]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(OfertaDetalheDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterDetalhesOferta(
        [FromRoute] Guid id,
        [FromServices] IConsultaOfertaDetalheService service,
        CancellationToken cancellationToken)
    {
        return await service.ConsultarAsync(id, cancellationToken).ToResponseResultAsync();
    }

    [HttpPost("gerar")]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> GerarLinkRastreavel(
        [FromBody] GerarLinkAfiliadoRequestDto request,
        [FromServices] IGerarLinkAfiliadoService service,
        CancellationToken ct)
    {
        var result = await service.GerarAsync(request, ct);
        return Ok(result);
    }
}