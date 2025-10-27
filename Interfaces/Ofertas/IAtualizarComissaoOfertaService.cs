using plataforma.ofertas._Base;

namespace plataforma.ofertas.Interfaces.Ofertas;

public interface IAtualizarComissaoOfertaService
{
    Task<CommandResult<Guid>> AtualizarAsync(Guid ofertaId, int novaComissao, CancellationToken ct);
}