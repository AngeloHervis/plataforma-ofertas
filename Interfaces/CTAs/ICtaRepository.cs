using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces.CTAs;

public interface ICtaRepository
{
    Task<List<Cta>> ObterCtasAsync(CancellationToken cancellationToken);
}