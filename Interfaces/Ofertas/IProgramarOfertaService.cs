using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IProgramarOfertaService
{
    Task<CommandResult<Guid>> ProgramarAsync(ProgramarOfertaRequestDto dto, CancellationToken ct);
}