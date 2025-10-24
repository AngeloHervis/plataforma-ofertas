using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Interfaces.Agendamentos;
using plataforma.ofertas.Interfaces.Ofertas;

namespace plataforma.ofertas.Services.Agendamentos;

public class ObterOfertaAgendadaDetalheService(IOfertaAgendadaRepository repo) : IObterOfertaAgendadaDetalheService
{
    public async Task<CommandResult<OfertaAgendadaDetalheDto>> ObterAsync(Guid id, CancellationToken ct)
    {
        var entidade = await repo.ObterPorIdAsync(id, ct);
        if (entidade is null)
            return CommandResult<OfertaAgendadaDetalheDto>.NotFound("Agendamento não encontrado.");

        return CommandResult<OfertaAgendadaDetalheDto>.Success(new OfertaAgendadaDetalheDto
        {
            Id = entidade.Id,
            Titulo = entidade.Titulo,
            Preco = entidade.Preco,
            Link = entidade.Link,
            ImagemUrl = entidade.ImagemUrl,
            Descricao = entidade.Descricao,
            DataHoraEnvio = entidade.DataHoraEnvio,
            Enviada = entidade.Enviada,
            CriadoEm = entidade.CriadoEm,
            AtualizadoEm = entidade.AtualizadoEm
        });
    }
}