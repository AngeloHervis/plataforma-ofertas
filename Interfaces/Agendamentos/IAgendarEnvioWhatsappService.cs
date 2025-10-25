using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces.Agendamentos;

public interface IAgendarEnvioWhatsappService
{
    Task<bool> AgendarImagemAsync(OfertaAgendada oferta, CancellationToken ct);
}