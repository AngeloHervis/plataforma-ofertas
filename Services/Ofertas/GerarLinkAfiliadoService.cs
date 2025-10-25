using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Interfaces.Scrapers;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Ofertas;

public class GerarLinkAfiliadoService(
    IAmazonScraperService amazonScraperService,
    IShopeeScraperService shopeeScraperService,
    IMercadoLivreScraperService mercadoLivreScraperService,
    IOfertaRepository ofertaRepository
) : IGerarLinkAfiliadoService
{
    private const string Fonte = "Usuario";

    public async Task<OfertaDetalheDto> GerarAsync(GerarLinkAfiliadoRequestDto request, CancellationToken ct)
    {
        if (request.TipoLink == "amazon")
        {
            var oferta = await amazonScraperService.ObterInformacoesCompletasDaAmazonAsync(request.Link, Fonte, ct);
            var dto = new OfertaDetalheDto
            {
                Fonte = Fonte,
                Titulo = oferta.Titulo,
                PrecoAtual = oferta.PrecoAtual,
                PrecoAnterior = oferta.PrecoAnterior,
                Link = oferta.Link,
                ImagemUrl = oferta.ImagemUrl
            };
            var id = await CadastrarOfertaNoBanco(dto, ct);
            if (id != Guid.Empty)
            {
                dto.Id = id;
                return dto;
            }
        }

        if (request.TipoLink == "shopee")
        {
            var oferta = await shopeeScraperService.ObterInformacoesCompletasDaShopeeAsync(request.Link, ct);
            var dto = new OfertaDetalheDto
            {
                Fonte = Fonte,
                Titulo = oferta.Titulo,
                PrecoAtual = oferta.PrecoAtual,
                PrecoAnterior = oferta.PrecoAnterior,
                Link = oferta.Link,
                ImagemUrl = oferta.ImagemUrl
            };
            var id = await CadastrarOfertaNoBanco(dto, ct);
            if (id != Guid.Empty)
            {
                dto.Id = id;
                return dto;
            }
        }

        if (request.TipoLink == "mercado-livre")
        {
            var oferta = await mercadoLivreScraperService.ObterInformacoesCompletasMercadoLivreAsync(request.Link, ct);
            var dto = new OfertaDetalheDto
            {
                Fonte = Fonte,
                Titulo = oferta.Titulo,
                PrecoAtual = oferta.PrecoAtual,
                PrecoAnterior = oferta.PrecoAnterior,
                Link = oferta.Link,
                ImagemUrl = oferta.ImagemUrl
            };
            var id = await CadastrarOfertaNoBanco(dto, ct);
            if (id != Guid.Empty)
            {
                dto.Id = id;
                return dto;
            }
        }
        return null;
    }
    
    private async Task<Guid> CadastrarOfertaNoBanco(OfertaDetalheDto ofertaDto, CancellationToken ct)
    {
        var oferta = new Oferta
        {
            Fonte = ofertaDto.Fonte,
            Titulo = ofertaDto.Titulo,
            PrecoAtual = ofertaDto.PrecoAtual,
            Link = ofertaDto.Link,
            ImagemUrl = ofertaDto.ImagemUrl,
            PublicadoEm = DateTime.UtcNow
        };
        var id = await ofertaRepository.CadastrarAsync(oferta, ct);
        return id;
    }
}