using plataforma.ofertas._Base;
using plataforma.ofertas.Interfaces.Ofertas;

namespace plataforma.ofertas.Services.Ofertas;

public class DeletarOfertaService(
    ILogger<DeletarOfertaService> logger,
    IOfertaRepository ofertaRepository
) : IDeletarOfertaService
{
    public async Task<CommandResult<Guid>> DeletarAsync(Guid id, CancellationToken ct)
    {
        try
        {
            var oferta =  await ofertaRepository.ObterPorIdAsync(id, ct);
            if (oferta == null)
            {
                logger.LogWarning("Oferta com Id: {Id} não encontrada para exclusão.", id);
                return CommandResult<Guid>.NotFound("Oferta não encontrada.");
            }
            
            await ofertaRepository.DeletarAsync(id, ct);
            logger.LogInformation("Exclusão da oferta com Id: {Id} finalizada.", id);
            return CommandResult<Guid>.Success(id);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Erro ao excluir oferta com Id: {Id}", id);
            throw;
        }
    }
}