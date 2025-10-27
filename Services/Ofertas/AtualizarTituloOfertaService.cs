using plataforma.ofertas._Base;
using plataforma.ofertas.Interfaces.Ofertas;

namespace plataforma.ofertas.Services.Ofertas;

public class AtualizarTituloOfertaService(IOfertaRepository ofertaRepository) : IAtualizarTituloOfertaService
{
    public async Task<CommandResult<Guid>> AtualizarAsync(Guid ofertaId, string novoTitulo, CancellationToken ct)
    {
        var oferta = await ofertaRepository.ObterPorIdAsync(ofertaId, ct);
        if (oferta == null)
            return CommandResult<Guid>.NotFound("Oferta não encontrada.");
        
        oferta.Titulo = novoTitulo;
        await ofertaRepository.AtualizarAsync(oferta, ct);
        
        return CommandResult<Guid>.Success(oferta.Id);
    }
}