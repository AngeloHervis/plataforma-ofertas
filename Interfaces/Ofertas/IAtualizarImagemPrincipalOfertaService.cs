using plataforma.ofertas._Base;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IAtualizarImagemPrincipalOfertaService
{
    Task<CommandResult<bool>> AtualizarAsync(Guid ofertaId, string novaImagemUrlPrincipal, CancellationToken ct);
}