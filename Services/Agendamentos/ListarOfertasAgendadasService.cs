using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Interfaces.Agendamentos;
using plataforma.ofertas.Interfaces.Ofertas;

namespace plataforma.ofertas.Services.Agendamentos;

public class ListarOfertasAgendadasService(IOfertaAgendadaRepository repo) : IListarOfertasAgendadasService
{
    public async Task<CommandResult<List<OfertaAgendadaResumoDto>>> ListarAsync(CancellationToken ct)
    {
        var itens = await repo.ListarTodasAsync(ct);

        var dto = itens
            .OrderBy(i => i.DataHoraEnvio)
            .Select(i => new OfertaAgendadaResumoDto
            {
                Id = i.Id,
                PrecoAnterior = i.PrecoAnterior,
                Link = i.Link,
                PorcentagemComissao = i.PorcentagemComissao,
                Titulo = i.Titulo,
                PrecoAtual = i.PrecoAtual,
                ImagemUrl = i.ImagemUrl,
                DataHoraAgendamento = i.DataHoraEnvio,
                Status = i.Status,
                CtaPersonalizado = i.Cta,
                Fonte = i.Fonte
            })
            .ToList();

        return dto.Count == 0 
            ? CommandResult<List<OfertaAgendadaResumoDto>>.Success([])
            : CommandResult<List<OfertaAgendadaResumoDto>>.Success(dto);
    }
}