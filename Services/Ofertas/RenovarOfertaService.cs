using plataforma.ofertas._Base;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Interfaces.Scrapers;

namespace plataforma.ofertas.Services.Ofertas;

public class RenovarOfertaService(
    IOfertaRepository ofertaRepository,
    IAmazonScraperService amazonScraperService
) : IRenovarOfertaService
{
    public async Task<CommandResult<Guid>> RenovarOfertaAsync(Guid ofertaId, CancellationToken cancellationToken)
    {
        var oferta = await ofertaRepository.ObterPorIdAsync(ofertaId, cancellationToken);
        if (oferta == null)
            return CommandResult<Guid>.NotFound("Oferta não encontrada.");

        var ofertaAtualizada =
            await amazonScraperService.ObterInformacoesCompletasDaAmazonAsync(oferta.Link, oferta.Fonte,
                cancellationToken);
        
        oferta.PrecoAnterior = ofertaAtualizada.PrecoAnterior;
        oferta.PrecoAtual = ofertaAtualizada.PrecoAtual;

        await ofertaRepository.AtualizarAsync(oferta, cancellationToken);

        return CommandResult<Guid>.Success(oferta.Id);
    }
}