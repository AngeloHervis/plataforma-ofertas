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
        const int limite = 20;
        var ofertas = await repo.ObterRecentesAsync(limite, ct);

        if (ofertas == null || ofertas.Count == 0)
            return CommandResult<List<OfertaResumoDto>>.NotFound(MensagensErro.NenhumaOfertaEncontrada);

        var dto = ofertas
            .OrderByDescending(o => o.PublicadoEm)
            .Select(o => new OfertaResumoDto {
                Id = o.Id,
                Fonte = o.Fonte,
                Titulo = o.Titulo,
                Preco = o.PrecoAtual,
                Link = o.Link,
                ImagemUrl = o.ImagemUrl
            })
            .ToList();

        return CommandResult<List<OfertaResumoDto>>.Success(dto);
    }
}