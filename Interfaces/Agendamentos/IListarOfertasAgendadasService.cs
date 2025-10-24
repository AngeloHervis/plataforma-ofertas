using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;

namespace plataforma.ofertas.Interfaces.Agendamentos;

public interface IListarOfertasAgendadasService
{
    Task<CommandResult<List<OfertaAgendadaResumoDto>>> ListarAsync(CancellationToken ct);
}