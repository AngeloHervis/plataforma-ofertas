using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Interfaces.Agendamentos;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Agendamentos;

public class AgendarOfertaService(IOfertaAgendadaRepository repo) : IAgendarOfertaService
{
    public async Task<CommandResult<OfertaAgendadaDetalheDto>> AgendarAsync(
        AgendarOfertaRequestDto request, CancellationToken ct)
    {
        var entidade = new OfertaAgendada
        {
            Titulo = request.CamposEditados.Titulo,
            Preco = request.CamposEditados.Preco,
            Link = request.CamposEditados.Link,
            ImagemUrl = request.CamposEditados.ImagemUrl,
            Descricao = request.CamposEditados.Descricao,
            DataHoraEnvio = request.DataHoraAgendamento,
            Enviada = false,
            CriadoEm = DateTime.UtcNow
        };

        var oferta = await repo.CriarAsync(entidade, ct);

        if (oferta is null)
            return CommandResult<OfertaAgendadaDetalheDto>.InternalError("Falha ao criar agendamento.");

        return CommandResult<OfertaAgendadaDetalheDto>.Success(new OfertaAgendadaDetalheDto
        {
            Id = oferta.Id,
            Titulo = oferta.Titulo,
            Preco = oferta.Preco,
            Link = oferta.Link,
            ImagemUrl = oferta.ImagemUrl,
            DataHoraEnvio = oferta.DataHoraEnvio,
            Enviada = oferta.Enviada,
            CriadoEm = oferta.CriadoEm,
            AtualizadoEm = oferta.AtualizadoEm
        });
    }
}