using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;

namespace plataforma.ofertas.Interfaces.Agendamentos;

public interface IAtualizarHorarioAgendamentoService
{
    Task<CommandResult<OfertaAgendadaDetalheDto>> AtualizarAsync(Guid id, DateTime novaDataHora, CancellationToken ct);
}