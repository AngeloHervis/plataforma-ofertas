using plataforma.ofertas.Dto.Agendamentos;

namespace plataforma.ofertas.Interfaces.Agendamentos;

public interface INovaPlataformaClient
{
    Task<Guid> EnviarOfertaAsync(IntegrarOfertaDto oferta, CancellationToken ct);
}