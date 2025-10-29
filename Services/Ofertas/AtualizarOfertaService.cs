using plataforma.ofertas._Base;
using plataforma.ofertas.Dto.Ofertas;
using plataforma.ofertas.Interfaces.Ofertas;

namespace plataforma.ofertas.Services.Ofertas;

public class AtualizarOfertaService(IOfertaRepository ofertaRepository) : IAtualizarOfertaService
{
    public async Task<CommandResult<bool>> AtualizarAsync(Guid ofertaId, AtualizarOfertaRequestDto request,
        CancellationToken ct)
    {
        var oferta = await ofertaRepository.ObterPorIdAsync(ofertaId, ct);
        if (oferta == null)
            return CommandResult<bool>.NotFound("Oferta não encontrada.");

        NormalizarPrecosRequest(request);

        oferta.Titulo = request.Titulo;
        oferta.PrecoAnterior = request.PrecoAnterior;
        oferta.PrecoAtual = request.PrecoAtual;

        await ofertaRepository.AtualizarAsync(oferta, ct);
        return CommandResult<bool>.Success(true);
    }

    private static void NormalizarPrecosRequest(AtualizarOfertaRequestDto request)
    {
        request.PrecoAnterior = NormalizarPreco(request.PrecoAnterior);
        request.PrecoAtual = NormalizarPreco(request.PrecoAtual);
    }

    private static string NormalizarPreco(string preco)
    {
        if (string.IsNullOrWhiteSpace(preco))
            return "R$0,00";

        if (!preco.StartsWith("R$"))
            preco = "R$" + preco;

        if (!preco.Contains(','))
            preco += ",00";

        return preco;
    }
}