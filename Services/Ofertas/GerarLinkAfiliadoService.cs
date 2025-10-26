using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Interfaces.Scrapers;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Ofertas;

public class GerarLinkAfiliadoService(
    IAmazonScraperService amazonScraperService,
    IShopeeScraperService shopeeScraperService,
    IMercadoLivreScraperService mercadoLivreScraperService,
    IOfertaRepository ofertaRepository,
    ILogger<GerarLinkAfiliadoService> logger
) : IGerarLinkAfiliadoService
{
    private const string Fonte = "Usuario";

    public async Task<OfertaDetalheDto> GerarAsync(GerarLinkAfiliadoRequestDto request, CancellationToken ct)
    {
        logger.LogInformation("Iniciando geração de link afiliado para tipo: {TipoLink}", request.TipoLink);

        if (request.TipoLink == "amazon")
        {
            logger.LogInformation("Processando link da Amazon: {Link}", request.Link);
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
                logger.LogInformation("Oferta da Amazon cadastrada com sucesso. Id: {Id}", id);
                dto.Id = id;
                return dto;
            }

            logger.LogWarning("Falha ao cadastrar oferta da Amazon.");
        }

        if (request.TipoLink == "shopee")
        {
            logger.LogInformation("Processando link da Shopee: {Link}", request.Link);
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
                logger.LogInformation("Oferta da Shopee cadastrada com sucesso. Id: {Id}", id);
                dto.Id = id;
                return dto;
            }

            logger.LogWarning("Falha ao cadastrar oferta da Shopee.");
        }

        if (request.TipoLink == "mercado-livre")
        {
            logger.LogInformation("Processando link do Mercado Livre: {Link}", request.Link);
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
                logger.LogInformation("Oferta do Mercado Livre cadastrada com sucesso. Id: {Id}", id);
                dto.Id = id;
                return dto;
            }

            logger.LogWarning("Falha ao cadastrar oferta do Mercado Livre.");
        }

        logger.LogWarning("Tipo de link não reconhecido ou falha ao gerar oferta.");
        return null;
    }

    private async Task<Guid> CadastrarOfertaNoBanco(OfertaDetalheDto ofertaDto, CancellationToken ct)
    {
        logger.LogInformation("Cadastrando oferta no banco: {Titulo}", ofertaDto.Titulo);
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
        logger.LogInformation("Cadastro realizado. Id: {Id}", id);
        return id;
    }
}