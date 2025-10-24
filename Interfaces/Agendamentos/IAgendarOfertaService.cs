using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;

namespace plataforma.ofertas.Interfaces.Agendamentos;

public interface IAgendarOfertaService
{
    Task<CommandResult<OfertaAgendadaDetalheDto>> AgendarAsync(AgendarOfertaRequestDto request, CancellationToken ct);
}