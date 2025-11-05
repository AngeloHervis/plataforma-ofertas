using Microsoft.Extensions.Options;
using plataforma.ofertas.Dto.Agendamentos;
using plataforma.ofertas.Interfaces.Agendamentos;

namespace plataforma.ofertas.Services.Agendamentos;

public sealed class NovaPlataformaClient(HttpClient http, IOptions<NovaPlataformaOptions> opts) : INovaPlataformaClient
{
    private readonly NovaPlataformaOptions _opts = opts.Value;

    public async Task<Guid> EnviarOfertaAsync(IntegrarOfertaDto oferta, CancellationToken ct)
    {
        try
        {
            using var response = await http.PostAsJsonAsync("integracao/receber-oferta", oferta, ct);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<OfertaRecebidaResponseDto>(cancellationToken: ct);
            if (result == null)
                throw new InvalidOperationException("Resposta inválida da Nova Plataforma");
            
            return result.OfertaId;
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Erro ao enviar oferta para a Nova Plataforma", e);
        }
    }
}