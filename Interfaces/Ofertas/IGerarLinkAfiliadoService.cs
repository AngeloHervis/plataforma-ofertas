using plataforma.ofertas.Dto.Ofertas;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IGerarLinkAfiliadoService
{
    Task<OfertaDetalheDto> GerarAsync(GerarLinkAfiliadoRequestDto request, CancellationToken ct);
}