using plataforma.ofertas._Base;
using plataforma.ofertas.Extensions;
using plataforma.ofertas.Interfaces.Ofertas;

namespace plataforma.ofertas.Services.Ofertas;

public class AdicionarImagemOfertaService(IOfertaRepository ofertaRepository) : IAdicionarImagemOfertaService
{
    public async Task<CommandResult<Guid>> AdicionarAsync(Guid ofertaId, string imagemUrl,
        CancellationToken cancellationToken)
    {
        var oferta = await ofertaRepository.ObterPorIdAsync(ofertaId, cancellationToken);
        if (oferta == null)
            return CommandResult<Guid>.NotFound("Oferta não encontrada.");

        if (string.IsNullOrWhiteSpace(imagemUrl))
            return CommandResult<Guid>.InvalidRequest("A URL da imagem não pode ser vazia.");

        var imagens = HelpersExtensions.ConverterStringParaLista(oferta.ImagensUrl);
        if (imagens.Contains(imagemUrl))
            return CommandResult<Guid>.InvalidRequest("A imagem já está associada a esta oferta.");

        imagens.Add(imagemUrl);
        oferta.ImagensUrl = HelpersExtensions.ConverterListaParaString(imagens);

        await ofertaRepository.AtualizarAsync(oferta, cancellationToken);
        return CommandResult<Guid>.Success(oferta.Id);
    }
}