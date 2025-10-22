using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Constantes;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Interfaces.Scrapers;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Ofertas;

public class ConsultaOfertasRecentesService(
    IPelandoScraperService pelandoScraperService,
    IPromobitScraperService promobitScraperService,
    IMercadoLivreScraperService mercadoLivreScraperService,
    IOfertaRepository ofertaRepository
) : IConsultaOfertasRecentesService
{
    public async Task<CommandResult<List<OfertaResumoDto>>> ConsultarAsync(CancellationToken ct)
    {
        try
        {
            var resultados = new List<CommandResult<List<Oferta>>>();
            await BuscarOfertasAsync(resultados, ct);

            var ofertasValidas = resultados
                .Where(r => !r.IsError && r.Data is not null)
                .SelectMany(r => r.Data!)
                .DistinctBy(o => new { o.Titulo, o.Link })
                .ToList();

            if (ofertasValidas.Count == 0)
                return CommandResult<List<OfertaResumoDto>>.NotFound(MensagensErro.NenhumaOfertaEncontrada);

            await ofertaRepository.SalvarVariasAsync(ofertasValidas, ct);

            var ofertasDto = ofertasValidas
                .OrderByDescending(o => o.PublicadoEm)
                .Select(o => new OfertaResumoDto
                {
                    Id = o.Id,
                    Fonte = o.Fonte,
                    Titulo = o.Titulo,
                    Preco = o.PrecoAtual,
                    Link = o.Link,
                    ImagemUrl = o.ImagemUrl
                })
                .ToList();

            return CommandResult<List<OfertaResumoDto>>.Success(ofertasDto);
        }
        catch (Exception ex)
        {
            return CommandResult<List<OfertaResumoDto>>.InternalError(
                $"{MensagensErro.FalhaConsultarOfertasRecentes}: {ex.Message}"
            );
        }
    }

    private async Task BuscarOfertasAsync(List<CommandResult<List<Oferta>>> resultados, CancellationToken ct)
    {
        // var pelandoTask = pelandoScraperService.ScrapePelandoAsync(ct);
        // var promobitTask = promobitScraperService.ScrapePromobitAsync(ct);
        //
        // await Task.WhenAll(pelandoTask, promobitTask);
        //
        // resultados.AddRange([pelandoTask.Result, promobitTask.Result]);
        var mercadoLivreTask = await mercadoLivreScraperService.ObterInformacoesCompletasMercadoLivreAsync(ct);
        resultados.Add(mercadoLivreTask);
    }
}
