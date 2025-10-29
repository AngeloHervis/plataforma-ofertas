using Microsoft.AspNetCore.Mvc;
using plataforma.ofertas.Dto.Constantes;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Extensions;
using plataforma.ofertas.Interfaces;
using plataforma.ofertas.Interfaces.Ofertas;

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

    [HttpPatch("{id:guid}")]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> AtualizarOferta(
        [FromRoute] Guid id,
        [FromBody] AtualizarOfertaRequestDto dto,
        [FromServices] IAtualizarOfertaService service,
        CancellationToken ct)
    {
        return await service.AtualizarAsync(id, dto, ct).ToResponseResultAsync();
    }

    [HttpPatch("{id:guid}/imagem-principal")]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> AtualizarImagemPrincipalOferta(
        [FromRoute] Guid id,
        [FromBody] AtualizarImagemRequestDto dto,
        [FromServices] IAtualizarImagemPrincipalOfertaService service,
        CancellationToken ct)
    {
        return await service.AtualizarAsync(id, dto.ImagemUrl, ct).ToResponseResultAsync();
    }

    [HttpPatch("{id:guid}/atualizar-titulo")]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> AtualizarTituloOferta(
        [FromRoute] Guid id,
        [FromBody] AtualizarTituloRequestDto dto,
        [FromServices] IAtualizarTituloOfertaService service,
        CancellationToken ct)
    {
        return await service.AtualizarAsync(id, dto.Titulo, ct).ToResponseResultAsync();
    }

    [HttpPatch("{id:guid}/adicionar-imagem")]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> AdicionarImagemOferta(
        [FromRoute] Guid id,
        [FromBody] AtualizarImagemRequestDto dto,
        [FromServices] IAdicionarImagemOfertaService service,
        CancellationToken ct)
    {
        return await service.AdicionarAsync(id, dto.ImagemUrl, ct).ToResponseResultAsync();
    }

    [HttpPatch("{id:guid}/remover-imagem")]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoverImagemOferta(
        [FromRoute] Guid id,
        [FromBody] AtualizarImagemRequestDto dto,
        [FromServices] IRemoverImagemOfertaService service,
        CancellationToken ct)
    {
        return await service.RemoverAsync(id, dto.ImagemUrl, ct).ToResponseResultAsync();
    }

    [HttpPatch("{id:guid}/comissao")]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> AtualizarComissaoOferta(
        [FromRoute] Guid id,
        [FromBody] AtualizarComissaoRequestDto dto,
        [FromServices] IAtualizarComissaoOfertaService service,
        CancellationToken ct)
    {
        return await service.AtualizarAsync(id, dto.PorcentagemComissao, ct).ToResponseResultAsync();
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
        return await service.GerarAsync(request, ct).ToResponseResultAsync();
    }

    [HttpDelete("remover/{id:guid}")]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeletarOferta(
        [FromRoute] Guid id,
        [FromServices] IDeletarOfertaService service,
        CancellationToken ct)
    {
        return await service.DeletarAsync(id, ct).ToResponseResultAsync();
    }

    [HttpPost("{id:guid}/renovar")]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> RenovarOferta(
        [FromRoute] Guid id,
        [FromServices] IRenovarOfertaService service,
        CancellationToken ct)
    {
        return await service.RenovarOfertaAsync(id, ct).ToResponseResultAsync();
    }
}