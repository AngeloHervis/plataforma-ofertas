using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface ICriarOfertaService
{
    Task<CommandResult<Guid>> CriarAsync(CriarOfertaRequestDto dto, CancellationToken ct);
}