using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IGerarLinkAfiliadoService
{
    Task<CommandResult<OfertaDetalheDto>> GerarAsync(GerarLinkAfiliadoRequestDto request, CancellationToken ct);
}