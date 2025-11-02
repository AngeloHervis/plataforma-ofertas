namespace plataforma.ofertas.Dto.Ofertas;

public class CriarOfertaRequestDto
{
    public string Titulo { get; init; }
    public string PrecoAtual { get; init; }
    public string PrecoAnterior { get; init; }
    public string LinkProduto { get; init; }
    public string ImagemUrl { get; init; }
    public int? PorcentagemComissao { get; init; }
    public string Cta { get; init; }
    public Guid TemplateId { get; init; }
    public string Cupom { get; init; }
    public bool TemCupom { get; init; }
}