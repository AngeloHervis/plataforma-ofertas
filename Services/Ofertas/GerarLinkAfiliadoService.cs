using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Extensions;
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
    public async Task<CommandResult<OfertaDetalheDto>> GerarAsync(GerarLinkAfiliadoRequestDto request, CancellationToken ct)
    {
        logger.LogInformation("Iniciando geração de link afiliado para tipo: {TipoLink}", request.TipoLink);

        if (request.TipoLink == "amazon")
        {
            logger.LogInformation("Processando link da Amazon: {Link}", request.Link);
            var oferta = await amazonScraperService.ObterInformacoesCompletasDaAmazonAsync(request.Link, "Amazon", ct);
            var dto = new OfertaDetalheDto
            {
                Fonte = "Amazon",
                Titulo = oferta.Titulo,
                PrecoAtual = oferta.PrecoAtual,
                PrecoAnterior = oferta.PrecoAnterior,
                Link = oferta.Link,
                ImagensUrl = HelpersExtensions.ConverterStringParaLista(oferta.ImagensUrl)
            };
            
            var id = await CadastrarOfertaNoBanco(oferta, ct);
            if (id != Guid.Empty)
            {
                logger.LogInformation("Oferta da Amazon cadastrada com sucesso. Id: {Id}", id);
                dto.Id = id;
                return CommandResult<OfertaDetalheDto>.Success(dto);
            }

            logger.LogWarning("Falha ao cadastrar oferta da Amazon.");
        }
        
        logger.LogWarning("Tipo de link não reconhecido ou falha ao gerar oferta.");
        return null;
    }

    private async Task<Guid> CadastrarOfertaNoBanco(Oferta oferta, CancellationToken ct)
    {
        logger.LogInformation("Cadastrando oferta no banco: {Titulo}", oferta.Titulo);
        var id = await ofertaRepository.CadastrarAsync(oferta, ct);
        logger.LogInformation("Cadastro realizado. Id: {Id}", id);
        return id;
    }
}
