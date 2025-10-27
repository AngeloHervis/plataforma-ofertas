using plataforma.ofertas.Interfaces.CTAs;
using plataforma.ofertas.Models;
using plataforma.ofertas.Storage;

namespace plataforma.ofertas.Repositories;

public class CtaRepository(SupabaseContext context) : ICtaRepository
{
    public async Task<List<Cta>> ObterCtasAsync(CancellationToken cancellationToken)
    {
        var client = context.Client;
        var response = await client
            .From<Cta>()
            .Get(cancellationToken: cancellationToken);

        return response.Models;
    }
}