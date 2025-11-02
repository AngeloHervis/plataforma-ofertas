using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Extensions;
using plataforma.ofertas.Interfaces.Agendamentos;
using plataforma.ofertas.Interfaces.CTAs;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Interfaces.Templates;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Ofertas;

public class ProgramarOfertaService(
    IOfertaRepository ofertaRepository,
    ICtaRepository ctaRepository,
    ITemplateRepository templateRepository,
    IAgendarEnvioWhatsappService agendarEnvioWhatsappService,
    IOfertaAgendadaRepository ofertaAgendadaRepository
) : IProgramarOfertaService
{
    public async Task<CommandResult<Guid>> ProgramarAsync(ProgramarOfertaRequestDto dto, CancellationToken ct)
    {
        var cta = await ctaRepository.ObterPorIdAsync(dto.CtaId, ct);
        if (cta == null)
            return CommandResult<Guid>.InvalidRequest($"CTA com ID {dto.CtaId} não encontrado.");

        var template = await templateRepository.ObterPorIdAsync(dto.TemplateId, ct);
        if (template == null)
            return CommandResult<Guid>.InvalidRequest($"Template com ID {dto.TemplateId} não encontrado.");

        var oferta = await CriarOfertaAsync(dto, ct);
        if (oferta == null)
            return CommandResult<Guid>.InternalError("Falha ao criar a oferta.");

        var ofertaAgendada = await CriarOfertaAgendadaAsync(dto, ct);
        if (ofertaAgendada == null)
            return CommandResult<Guid>.InternalError("Falha ao agendar a oferta.");

        var agendamento = await agendarEnvioWhatsappService.AgendarImagemAsync(ofertaAgendada, ct);
        if (!agendamento)
            return CommandResult<Guid>.InternalError("Falha ao agendar o envio da oferta.");

        ofertaAgendada.Status = "enviada";
        ofertaAgendada.AtualizadoEm = DateTime.UtcNow;
        await ofertaAgendadaRepository.AtualizarAsync(ofertaAgendada, ct);
        
        return CommandResult<Guid>.Success(ofertaAgendada.Id);
    }

    private async Task<Oferta> CriarOfertaAsync(ProgramarOfertaRequestDto dto, CancellationToken ct)
    {
        var oferta = new Oferta
        {
            Titulo = dto.Titulo,
            PrecoAtual = dto.PrecoAtual.PadronizarPreco(),
            Fonte = HelpersExtensions.ExtrairFonteDaUrl(dto.LinkProduto),
            PrecoAnterior = dto.PrecoAnterior.PadronizarPreco(),
            Link = dto.LinkProduto,
            ImagemUrlPrincipal = dto.ImagemUrl,
            ImagensUrl = dto.ImagemUrl,
            DescontoPercentual = HelpersExtensions.CalcularPercentual(dto.PrecoAtual, dto.PrecoAnterior),
            PorcentagemComissao = dto.PorcentagemComissao,
            CtaId = dto.CtaId,
            TemplateId = dto.TemplateId,
            Cupom = dto.Cupom,
            TemCupom = dto.TemCupom,
            PublicadoEm = DateTime.UtcNow
        };

        var id = await ofertaRepository.CadastrarAsync(oferta, ct);
        return id != Guid.Empty ? oferta : null;
    }

    private async Task<OfertaAgendada> CriarOfertaAgendadaAsync(ProgramarOfertaRequestDto dto, CancellationToken ct)
    {
        var ofertaAgendada = new OfertaAgendada
        {
            Titulo = dto.Titulo,
            PrecoAtual = dto.PrecoAtual.PadronizarPreco(),
            PrecoAnterior = dto.PrecoAnterior.PadronizarPreco(),
            Link = dto.LinkProduto,
            ImagemUrl = dto.ImagemUrl,
            Status = "pendente",
            PorcentagemComissao = dto.PorcentagemComissao,
            CtaId = dto.CtaId,
            TemplateId = dto.TemplateId,
            Cupom = dto.Cupom,
            TemCupom = dto.TemCupom,
            DataHoraEnvio = dto.DataHoraAgendamento
        };

        var ofertaAgendadaNoBanco = await ofertaAgendadaRepository.CriarAsync(ofertaAgendada, ct);
        return ofertaAgendadaNoBanco;
    }
}