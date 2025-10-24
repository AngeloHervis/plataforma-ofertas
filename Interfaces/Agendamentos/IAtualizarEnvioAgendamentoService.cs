using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Agendamentos;
using plataforma.ofertas.Dto.Ofertas;

namespace plataforma.ofertas.Interfaces.Agendamentos;

public interface IAtualizarEnvioAgendamentoService
{
    Task<CommandResult<OfertaAgendadaDetalheDto>> AtualizarAsync(Guid id, AtualizarEnvioDto dto, CancellationToken ct);
}