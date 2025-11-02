using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Extensions;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Models;

namespace plataforma.ofertas.Services.Ofertas;

public class CriarOfertaService(IOfertaRepository ofertaRepository) : ICriarOfertaService
{
    public async Task<CommandResult<Guid>> CriarAsync(CriarOfertaRequestDto dto, CancellationToken ct)
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
            DescontoPercentual = HelpersExtensions.CalcularPercentual(dto.PrecoAtual,  dto.PrecoAnterior),
            PorcentagemComissao = dto.PorcentagemComissao,
            CtaId = dto.CtaId,
            TemplateId = dto.TemplateId,
            Cupom = dto.Cupom,
            TemCupom = dto.TemCupom,
            PublicadoEm = DateTime.UtcNow
        };

        var id = await ofertaRepository.CadastrarAsync(oferta, ct);
        return id != Guid.Empty
            ? CommandResult<Guid>.Success(id)
            : CommandResult<Guid>.InternalError("Falha ao criar a oferta.");
    }
}