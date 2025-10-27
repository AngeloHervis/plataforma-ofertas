using Microsoft.AspNetCore.Mvc;
using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Constantes;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Extensions;
using plataforma.ofertas.Interfaces.Agendamentos;

namespace plataforma.ofertas.Controllers;

[ApiController]
[Route("api/ofertas/agendadas")]
public class OfertasAgendadasController : ControllerBase
{
    [HttpPost("agendar")]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(OfertaAgendadaDetalheDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> AgendarOferta(
        [FromBody] AgendarOfertaRequestDto request,
        [FromServices] IAgendarOfertaService service,
        CancellationToken ct)
    {
        return await service.AgendarAsync(request, ct).ToResponseResultAsync();
    }

    [HttpGet]
    [Produces(TiposRequisicaoERetorno.JsonText)]
    [ProducesResponseType(typeof(List<OfertaAgendadaResumoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarAgendadas(
        [FromServices] IListarOfertasAgendadasService service,
        CancellationToken ct)
    {
        return await service.ListarAsync(ct).ToResponseResultAsync();
    }
}