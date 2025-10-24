using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Agendamentos;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Interfaces.Agendamentos;
using plataforma.ofertas.Interfaces.Ofertas;

namespace plataforma.ofertas.Services.Agendamentos;

public class AtualizarEnvioAgendamentoService(IOfertaAgendadaRepository repo) : IAtualizarEnvioAgendamentoService
{
    public async Task<CommandResult<OfertaAgendadaDetalheDto>> AtualizarAsync(
        Guid id, AtualizarEnvioDto dto, CancellationToken ct)
    {
        var atual = await repo.ObterPorIdAsync(id, ct);
        if (atual is null)
            return CommandResult<OfertaAgendadaDetalheDto>.NotFound("Agendamento não encontrado.");

        atual.Titulo = dto.Titulo;
        atual.Preco = dto.Preco;
        atual.Link = dto.Link;
        atual.ImagemUrl = dto.ImagemUrl;
        atual.Descricao = dto.Descricao;
        atual.DataHoraEnvio = dto.DataHoraEnvio;
        atual.Enviada = dto.Enviada;
        atual.AtualizadoEm = DateTime.UtcNow;

        await repo.AtualizarAsync(atual, ct);

        var atualizado = await repo.ObterPorIdAsync(id, ct);
        if (atualizado is null)
            return CommandResult<OfertaAgendadaDetalheDto>.InternalError("Falha ao salvar alterações.");

        return CommandResult<OfertaAgendadaDetalheDto>.Success(new OfertaAgendadaDetalheDto
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