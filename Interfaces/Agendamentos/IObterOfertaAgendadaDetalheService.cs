using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;

namespace plataforma.ofertas.Interfaces.Agendamentos;

public interface IObterOfertaAgendadaDetalheService
{
    Task<CommandResult<OfertaAgendadaDetalheDto>> ObterAsync(Guid id, CancellationToken ct);
}