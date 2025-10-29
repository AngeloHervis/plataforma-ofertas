using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IAtualizarOfertaService
{
    Task<CommandResult<bool>> AtualizarAsync(Guid ofertaId, AtualizarOfertaRequestDto request, CancellationToken ct);
}