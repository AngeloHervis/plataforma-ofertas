using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Constantes;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Ofertas;

public class ConsultaOfertasDoBancoService(IOfertaRepository repo) : IConsultaOfertasDoBancoService
{
    public async Task<CommandResult<List<OfertaResumoDto>>> ConsultarAsync(CancellationToken ct)
    {
        var ofertas = await repo.ObterRecentesAsync(ct);

        if (ofertas == null || ofertas.Count == 0)
            return CommandResult<List<OfertaResumoDto>>.NotFound(MensagensErro.NenhumaOfertaEncontrada);

        var dto = ofertas
            .OrderByDescending(o => o.PublicadoEm)
            .Select(o => new OfertaResumoDto {
                Id = o.Id,
                Fonte = o.Fonte,
                Titulo = o.Titulo,
                Preco = o.PrecoAtual,
                PrecoAnterior = o.PrecoAnterior,
                Link = o.Link,
                ImagemUrl = o.ImagemUrlPrincipal,
                PorcentagemComissao = o.PorcentagemComissao
            })
            .ToList();

        return CommandResult<List<OfertaResumoDto>>.Success(dto);
    }
}