using plataforma.ofertas._Base;
using plataforma.ofertas.Interfaces.Ofertas;

namespace plataforma.ofertas.Services.Ofertas;

public class AtualizarImagemPrincipalOfertaService(IOfertaRepository ofertaRepository) : IAtualizarImagemPrincipalOfertaService
{
    public async Task<CommandResult<bool>> AtualizarAsync(Guid ofertaId, string novaImagemUrlPrincipal, CancellationToken ct)
    {
        var oferta = await ofertaRepository.ObterPorIdAsync(ofertaId, ct);
        if (oferta == null)
            return CommandResult<bool>.NotFound("Oferta não encontrada.");

        oferta.ImagemUrlPrincipal = novaImagemUrlPrincipal;
        await ofertaRepository.AtualizarAsync(oferta, ct);

        return CommandResult<bool>.Success(true);
    }
}