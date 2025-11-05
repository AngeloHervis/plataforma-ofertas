using System.Text.RegularExpressions;
using plataforma.ofertas.Dto.Agendamentos;
using plataforma.ofertas.Extensions;
using plataforma.ofertas.Interfaces.Agendamentos;
using plataforma.ofertas.Interfaces.Templates;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Agendamentos;

public sealed class AgendarEnvioWhatsappService(
    ISendFlowActionsClient client,
    ITemplateRepository templateRepository,
    INovaPlataformaClient novaPlataformaClient
) : IAgendarEnvioWhatsappService
{
    public async Task<bool> AgendarImagemAsync(OfertaAgendada oferta, CancellationToken ct)
    {
        var whenUtc = oferta.DataHoraEnvio.ToUniversalTime();
        var link = await EnviarParaNovaPlataforma(oferta, ct);
        var caption = await MontarCaption(oferta, link, ct);

        return await client.ScheduleImageToReleaseAsync(
            caption: caption,
            imageUrl: oferta.ImagemUrlWhatsapp!,
            scheduledAtUtc: whenUtc,
            ct: ct
        );
    }

    private async Task<string> EnviarParaNovaPlataforma(OfertaAgendada oferta, CancellationToken ct)
    {
        var integrarDto = new IntegrarOfertaDto
        {
            Titulo = oferta.Titulo ?? string.Empty,
            Descricao = oferta.Descricao ?? string.Empty,
            PrecoAtual = oferta.PrecoAtual.PadronizarPreco() ?? string.Empty,
            PrecoAnterior = oferta.PrecoAnterior.PadronizarPreco() ?? string.Empty,
            ImagemUrlPrincipal = oferta.ImagemUrlSite ?? string.Empty,
            Marketplace = oferta.Fonte ?? string.Empty,
            UrlOrigem = oferta.Link ?? string.Empty
        };

        var idOferta = await novaPlataformaClient.EnviarOfertaAsync(integrarDto, ct);
        const string baseUrl = "https://beloto-ofertas.vercel.app/oferta/";
        oferta.Link = $"{baseUrl}{idOferta}";
        return oferta.Link;
    }

    private async Task<string> MontarCaption(OfertaAgendada oferta, string link, CancellationToken ct)
    {
        var template = await templateRepository.ObterPorIdAsync(oferta.TemplateId, ct);

        if (template == null)
            throw new InvalidOperationException($"Template com ID {oferta.TemplateId} não encontrado");

        var caption = SubstituirVariaveisTemplate(template.Conteudo, oferta, link);

        return caption;
    }

    private static string SubstituirVariaveisTemplate(string templateConteudo, OfertaAgendada oferta, string link)
    {
        var caption = templateConteudo;

        caption = Regex.Replace(caption, @"\{\{cta\}\}", oferta.Cta ?? string.Empty, RegexOptions.IgnoreCase);
        caption = Regex.Replace(caption, @"\{\{titulo\}\}", oferta.Titulo ?? string.Empty, RegexOptions.IgnoreCase);
        caption = Regex.Replace(caption, @"\{\{preco-anterior\}\}", oferta.PrecoAnterior.PadronizarPreco() ?? string.Empty, RegexOptions.IgnoreCase);
        caption = Regex.Replace(caption, @"\{\{preco-atual\}\}", oferta.PrecoAtual.PadronizarPreco() ?? string.Empty, RegexOptions.IgnoreCase);
        caption = Regex.Replace(caption, @"\{\{link\}\}", link ?? string.Empty, RegexOptions.IgnoreCase);
        caption = Regex.Replace(caption, @"\{\{cupom\}\}", oferta.TemCupom ? (oferta.Cupom ?? string.Empty) : "Sem cupom", RegexOptions.IgnoreCase);
        caption = Regex.Replace(caption, @"\n\s*\n\s*\n", "\n\n");

        return caption.Trim();
    }
}