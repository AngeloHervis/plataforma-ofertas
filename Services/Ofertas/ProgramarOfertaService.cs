using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Extensions;
using plataforma.ofertas.Interfaces.Agendamentos;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Interfaces.Templates;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Ofertas;

public class ProgramarOfertaService(
    ITemplateRepository templateRepository,
    IAgendarEnvioWhatsappService agendarEnvioWhatsappService,
    IOfertaAgendadaRepository ofertaAgendadaRepository
) : IProgramarOfertaService
{
    public async Task<CommandResult<Guid>> ProgramarAsync(ProgramarOfertaRequestDto dto, CancellationToken ct)
    {
        var template = await templateRepository.ObterPorIdAsync(dto.TemplateId, ct);
        if (template == null)
            return CommandResult<Guid>.InvalidRequest($"Template com ID {dto.TemplateId} não encontrado.");

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

    private async Task<OfertaAgendada> CriarOfertaAgendadaAsync(ProgramarOfertaRequestDto dto, CancellationToken ct)
    {
        var ofertaAgendada = new OfertaAgendada
        {
            Titulo = dto.Titulo,
            PrecoAtual = dto.PrecoAtual.PadronizarPreco(),
            PrecoAnterior = dto.PrecoAnterior.PadronizarPreco(),
            Link = dto.LinkProduto,
            ImagemUrl = dto.ImagemUrlSite,
            Status = "pendente",
            PorcentagemComissao = dto.PorcentagemComissao,
            Descricao = dto.Descricao,
            Cta = dto.Cta,
            ImagemUrlSite = dto.ImagemUrlSite,
            ImagemUrlWhatsapp = dto.ImagemUrlWhatsapp,
            Fonte = HelpersExtensions.ExtrairFonteDaUrl(dto.LinkProduto),
            TemplateId = dto.TemplateId,
            Cupom = dto.Cupom,
            TemCupom = dto.TemCupom,
            DataHoraEnvio = dto.DataHoraAgendamento
        };

        var ofertaAgendadaNoBanco = await ofertaAgendadaRepository.CriarAsync(ofertaAgendada, ct);
        return ofertaAgendadaNoBanco;
    }
}