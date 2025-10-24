using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Agendamentos;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Interfaces.Agendamentos;
using plataforma.ofertas.Interfaces.Ofertas;

namespace plataforma.ofertas.Services.Agendamentos;

public class AtualizarHorarioAgendamentoService(IOfertaAgendadaRepository repo) : IAtualizarHorarioAgendamentoService
{
    public async Task<CommandResult<OfertaAgendadaDetalheDto>> AtualizarAsync(
        Guid id, DateTime novaDataHora, CancellationToken ct)
    {
        var atual = await repo.ObterPorIdAsync(id, ct);
        if (atual is null)
            return CommandResult<OfertaAgendadaDetalheDto>.NotFound("Agendamento não encontrado.");

        await repo.AtualizarHorarioAsync(id, novaDataHora, ct);
        var atualizado = await repo.ObterPorIdAsync(id, ct);

        return atualizado is null
            ? CommandResult<OfertaAgendadaDetalheDto>.InternalError("Falha ao atualizar horário.")
            : CommandResult<OfertaAgendadaDetalheDto>.Success(new OfertaAgendadaDetalheDto
            {
                Id = atualizado.Id,
                Titulo = atualizado.Titulo,
                Preco = atualizado.Preco,
                Link = atualizado.Link,
                ImagemUrl = atualizado.ImagemUrl,
                Descricao = atualizado.Descricao,
                DataHoraEnvio = atualizado.DataHoraEnvio,
                Enviada = atualizado.Enviada,
                CriadoEm = atualizado.CriadoEm,
                AtualizadoEm = atualizado.AtualizadoEm
            });
    }
}