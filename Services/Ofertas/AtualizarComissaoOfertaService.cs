using plataforma.ofertas._Base;
using plataforma.ofertas.Interfaces.Ofertas;

namespace plataforma.ofertas.Services.Ofertas;

public class AtualizarComissaoOfertaService(IOfertaRepository ofertaRepository) : IAtualizarComissaoOfertaService
{
    public async Task<CommandResult<Guid>> AtualizarAsync(Guid ofertaId, int novaComissao, CancellationToken ct)
    {
        var oferta = await ofertaRepository.ObterPorIdAsync(ofertaId, ct);
        if (oferta == null)
            return CommandResult<Guid>.NotFound("Oferta não encontrada.");
        
        oferta.PorcentagemComissao = novaComissao;
        await ofertaRepository.AtualizarAsync(oferta, ct);
        
        return CommandResult<Guid>.Success(oferta.Id);
    }
}