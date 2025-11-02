using plataforma.ofertas.Interfaces.Agendamentos;
using plataforma.ofertas.Interfaces.CTAs;
using plataforma.ofertas.Interfaces.Templates;
using plataforma.ofertas.Models;
using System.Text.RegularExpressions;
using plataforma.ofertas.Extensions;

namespace plataforma.ofertas.Services.Agendamentos;

public sealed class AgendarEnvioWhatsappService(
    ISendFlowActionsClient client,
    ITemplateRepository templateRepository,
    ICtaRepository ctaRepository
) : IAgendarEnvioWhatsappService
{
    public async Task<bool> AgendarImagemAsync(OfertaAgendada oferta, CancellationToken ct)
    {
        var caption = await MontarCaption(oferta, ct);
        var whenUtc = oferta.DataHoraEnvio.ToUniversalTime();

        return await client.ScheduleImageToReleaseAsync(
            caption: caption,
            imageUrl: oferta.ImagemUrl!,
            scheduledAtUtc: whenUtc,
            ct: ct
        );
    }

    private async Task<string> MontarCaption(OfertaAgendada oferta, CancellationToken ct)
    {
        var template = await templateRepository.ObterPorIdAsync(oferta.TemplateId, ct);
        var cta = await ctaRepository.ObterPorIdAsync(oferta.CtaId, ct);

        if (template == null)
            throw new InvalidOperationException($"Template com ID {oferta.TemplateId} não encontrado");

        var caption = SubstituirVariaveisTemplate(template.Conteudo, oferta, cta);

        return caption;
    }

    private static string SubstituirVariaveisTemplate(string templateConteudo, OfertaAgendada oferta, Cta cta)
    {
        var caption = templateConteudo;

        caption = Regex.Replace(caption, @"\{\{cta\}\}", cta.Titulo ?? string.Empty, RegexOptions.IgnoreCase);
        caption = Regex.Replace(caption, @"\{\{titulo\}\}", oferta.Titulo ?? string.Empty, RegexOptions.IgnoreCase);
        caption = Regex.Replace(caption, @"\{\{preco-anterior\}\}", oferta.PrecoAnterior.PadronizarPreco() ?? string.Empty, RegexOptions.IgnoreCase);
        caption = Regex.Replace(caption, @"\{\{preco-atual\}\}", oferta.PrecoAtual.PadronizarPreco() ?? string.Empty, RegexOptions.IgnoreCase);
        caption = Regex.Replace(caption, @"\{\{link\}\}", oferta.Link ?? string.Empty, RegexOptions.IgnoreCase);
        caption = Regex.Replace(caption, @"\{\{cupom\}\}", oferta.TemCupom ? (oferta.Cupom ?? string.Empty) : "Sem cupom", RegexOptions.IgnoreCase);
        caption = Regex.Replace(caption, @"\n\s*\n\s*\n", "\n\n");

        return caption.Trim();
    }
}