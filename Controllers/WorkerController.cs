using Microsoft.AspNetCore.Mvc;
using plataforma.ofertas.Services.Jobs;

namespace plataforma.ofertas.Controllers;

[ApiController]
[Route("api/worker")]
public class WorkerController(IJobRegistry registry) : ControllerBase
{
    /// <summary>Executa um serviço de worker uma única vez (teste manual).</summary>
    /// <param name="service">Nome do serviço (ex: atualizar-ofertas)</param>
    [HttpPost("run")]
    public async Task<IActionResult> Run([FromQuery] string service, CancellationToken ct)
    {
        var job = registry.Resolve(service);
        if (job is null) return NotFound(new { message = $"Serviço '{service}' não encontrado." });

        await job.RunAsync(ct);
        return Ok(new { message = $"Serviço '{service}' executado com sucesso." });
    }
}