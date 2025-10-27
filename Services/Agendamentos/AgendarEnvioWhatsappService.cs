using plataforma.ofertas.Interfaces.Agendamentos;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Agendamentos;

public sealed class AgendarEnvioWhatsappService(ISendFlowActionsClient client) : IAgendarEnvioWhatsappService
{
    public async Task<bool> AgendarImagemAsync(OfertaAgendada oferta, CancellationToken ct)
    {
        var caption = MontarCaption(oferta);
        var whenUtc = oferta.DataHoraEnvio.ToUniversalTime();

        return await client.ScheduleImageToReleaseAsync(
            caption: caption,
            imageUrl: oferta.ImagemUrl!,
            scheduledAtUtc: whenUtc,
            ct: ct
        );
    }

    private static string MontarCaption(OfertaAgendada oferta)
    {
        var linhasPreco = ValidarPrecosMensagem(oferta);

        var captionLines = new List<string>
        {
            $"*{oferta.Cta}*",
            "",
            $"_{oferta.Titulo}_"
        };

        captionLines.AddRange(linhasPreco);

        if (!string.IsNullOrWhiteSpace(oferta.Link))
        {
            captionLines.Add("");
            captionLines.Add("👇 Válido somente aqui:");
            captionLines.Add(oferta.Link);
        }

        return string.Join("\n", captionLines);
    }

    private static List<string> ValidarPrecosMensagem(OfertaAgendada oferta)
    {
        var temPrecoAnterior = !string.IsNullOrWhiteSpace(oferta.PrecoAnterior);

        var linhasPreco = new List<string>();
        if (temPrecoAnterior)
        {
            linhasPreco.Add($"De: ~{oferta.PrecoAnterior}~");
            linhasPreco.Add($"Por: *{oferta.PrecoAtual}*");
            return linhasPreco;
        }

        linhasPreco.Add($"*{oferta.PrecoAtual}*");
        return linhasPreco;
    }
}