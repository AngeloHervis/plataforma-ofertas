using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Models;
using plataforma.ofertas.Storage;
using Supabase.Postgrest;
using Client = Supabase.Client;

namespace plataforma.ofertas.Repositories;

public class OfertaAgendadaRepository(SupabaseContext context) : IOfertaAgendadaRepository
{
    private readonly Client _client = context.Client;

    public async Task<OfertaAgendada> CriarAsync(OfertaAgendada entidade, CancellationToken ct)
    {
        if (entidade.Id == Guid.Empty) entidade.Id = Guid.NewGuid();
        entidade.CriadoEm = DateTime.UtcNow;
        var response = await _client.From<OfertaAgendada>().Insert(entidade, cancellationToken: ct);
        return response.Models.FirstOrDefault();
    }

    public async Task<List<OfertaAgendada>> ListarTodasAsync(CancellationToken ct)
    {
        var resp = await _client
            .From<OfertaAgendada>()
            .Order("data_hora_envio", Constants.Ordering.Ascending)
            .Get(ct);
        return resp.Models;
    }

    public async Task<OfertaAgendada> ObterPorIdAsync(Guid id, CancellationToken ct)
    {
        var resp = await _client
            .From<OfertaAgendada>()
            .Filter("id", Constants.Operator.Equals, id.ToString())
            .Get(ct);

        return resp.Models.FirstOrDefault();
    }

    public async Task AtualizarAsync(OfertaAgendada entidade, CancellationToken ct)
    {
        entidade.AtualizadoEm = DateTime.UtcNow;
        await _client.From<OfertaAgendada>().Update(entidade, cancellationToken: ct);
    }

    public async Task AtualizarHorarioAsync(Guid id, DateTime novaDataHora, CancellationToken ct)
    {
        var partial = new OfertaAgendada
        {
            Id = id,
            DataHoraEnvio = novaDataHora,
            AtualizadoEm = DateTime.UtcNow
        };

        await _client.From<OfertaAgendada>().Update(partial, cancellationToken: ct);
    }
}