using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Interfaces.Ofertas;

namespace plataforma.ofertas.Services.Ofertas;

public class ConsultaOfertaDetalheService(IOfertaRepository repo) : IConsultaOfertaDetalheService
{
    public async Task<CommandResult<OfertaDetalheDto>> ConsultarAsync(Guid id, CancellationToken cancellationToken)
    {
        var oferta = await repo.ObterPorIdAsync(id, cancellationToken);
        if (oferta == null)
            return CommandResult<OfertaDetalheDto>.NotFound("Oferta não encontrada.");

        var dto = new OfertaDetalheDto
        {
            Id = oferta.Id,
            Fonte = oferta.Fonte,
            Titulo = oferta.Titulo,
            PrecoAnterior = oferta.PrecoAnterior,
            PrecoAtual = oferta.PrecoAtual,
            Link = oferta.Link,
            ImagemUrl = oferta.ImagemUrl,
            PublicadoEm = oferta.PublicadoEm
        };
        return CommandResult<OfertaDetalheDto>.Success(dto);
    }
}