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
        var frasesAbertura = new[]
        {
            "🔥 Achado do dia!",
            "💥 Olha essa oferta absurda!",
            "🚨 Promoção relâmpago detectada!",
            "🤑 Preço baixou demais!",
            "🎯 Oferta certeira pra você!",
            "💣 Tá quase de graça!",
            "🧨 Achado imperdível!",
            "📉 Caiu o preço!",
            "⚡ Não dura muito tempo!",
            "🏷️ Desconto real oficial!",
            "💰 Economize agora!",
            "🤯 Inacreditável esse valor!",
            "📦 Últimas unidades com desconto!",
            "💎 Achado premium!",
            "🚀 Aproveita antes que acabe!",
            "🔥 Tá voando das prateleiras!",
            "💸 Promoção que vale ouro!",
            "🏆 Oferta destaque do dia!",
            "🎁 Desconto escondido revelado!",
            "📲 Clique antes que suba o preço!",
            "🧠 Inteligente é quem aproveita!",
            "😱 Olha o preço disso!",
            "💥 Explosão de descontos!",
            "📢 Promo boa a gente compartilha!",
            "🎯 Certeiro pra quem ama economia!",
            "📉 Baixou de novo!",
            "💥 Oferta que não dá pra ignorar!",
            "🚨 Última chamada!",
            "🛒 Corre garantir o seu!",
            "🔥 Queima de estoque!"
        };
        var random = new Random();
        var fraseEscolhida = frasesAbertura[random.Next(frasesAbertura.Length)];

        var linhasPreco = ValidarPrecosMensagem(oferta);

        var captionLines = new List<string>
        {
            $"*{fraseEscolhida}*",
            "",
            $"🔖 {oferta.Titulo}"
        };

        captionLines.AddRange(linhasPreco);

        captionLines.AddRange(new[]
        {
            "",
            !string.IsNullOrWhiteSpace(oferta.Link) ? $"🔗 {oferta.Link}" : null,
            "",
            "_Compartilhe com alguém que vai gostar!_"
        });

        return string.Join("\n", captionLines.Where(l => !string.IsNullOrWhiteSpace(l)));
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