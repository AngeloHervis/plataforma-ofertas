using plataforma.ofertas._Base;
using plataforma.ofertas.Extensions;
using plataforma.ofertas.Interfaces.Ofertas;

namespace plataforma.ofertas.Services.Ofertas;

public class RemoverImagemOfertaService(IOfertaRepository ofertaRepository) : IRemoverImagemOfertaService
{
    public async Task<CommandResult<Guid>> RemoverAsync(Guid ofertaId, string imagemUrl,
        CancellationToken cancellationToken)
    {
        var oferta = await ofertaRepository.ObterPorIdAsync(ofertaId, cancellationToken);
        if (oferta == null)
            return CommandResult<Guid>.NotFound("Oferta não encontrada.");

        if (string.IsNullOrWhiteSpace(imagemUrl))
            return CommandResult<Guid>.InvalidRequest("A URL da imagem não pode ser vazia.");

        var imagens = HelpersExtensions.ConverterStringParaLista(oferta.ImagensUrl);
        if (!imagens.Contains(imagemUrl))
            return CommandResult<Guid>.InvalidRequest("A imagem especificada não está associada a esta oferta.");

        imagens.Remove(imagemUrl);
        oferta.ImagensUrl = HelpersExtensions.ConverterListaParaString(imagens);

        if (oferta.ImagemUrlPrincipal == imagemUrl)
            oferta.ImagemUrlPrincipal = string.Empty;

        await ofertaRepository.AtualizarAsync(oferta, cancellationToken);
        return CommandResult<Guid>.Success(oferta.Id);
    }
}