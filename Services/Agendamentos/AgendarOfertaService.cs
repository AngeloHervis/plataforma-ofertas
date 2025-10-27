using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Interfaces.Agendamentos;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Agendamentos;

public class AgendarOfertaService(IOfertaAgendadaRepository repo, IAgendarEnvioWhatsappService agendarEnvioWhatsappService) : IAgendarOfertaService
{
    public async Task<CommandResult<OfertaAgendadaDetalheDto>> AgendarAsync(
        AgendarOfertaRequestDto request, CancellationToken ct)
    {
        var entidade = new OfertaAgendada
        {
            Titulo = request.Titulo,
            PrecoAtual = request.PrecoAtual,
            PrecoAnterior = request.PrecoAnterior,
            Link = request.Link,
            ImagemUrl = request.ImagemUrl,
            DataHoraEnvio = request.DataHoraAgendamento,
            Status = "Pendente",
            PorcentagemComissao = request.PorcentagemComissao,
            Cta = request.CtaPersonalizado,
            CriadoEm = DateTime.UtcNow
        };

        var oferta = await repo.CriarAsync(entidade, ct);
        if (oferta is null)
            return CommandResult<OfertaAgendadaDetalheDto>.InternalError("Falha ao criar agendamento no banco.");
        
        var agendamentoConcluido = await agendarEnvioWhatsappService.AgendarImagemAsync(oferta, ct);

        if (!agendamentoConcluido)
            return CommandResult<OfertaAgendadaDetalheDto>.InternalError("Falha ao criar agendamento no sendflow.");

        return CommandResult<OfertaAgendadaDetalheDto>.Success(new OfertaAgendadaDetalheDto
        {
            Id = oferta.Id,
            Titulo = oferta.Titulo,
            PrecoAtual = oferta.PrecoAtual,
            PrecoAnterior = oferta.PrecoAnterior,
            Link = oferta.Link,
            ImagemUrl = oferta.ImagemUrl,
            DataHoraEnvio = oferta.DataHoraEnvio,
            Status = "Pendente",
            CriadoEm = oferta.CriadoEm,
            AtualizadoEm = oferta.AtualizadoEm
        });
    }
}