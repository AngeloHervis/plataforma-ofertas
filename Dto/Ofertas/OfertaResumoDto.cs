namespace plataforma.ofertas.Dto.Ofertas;

public record OfertaResumoDto
{
    public Guid Id { get; init; }
    public string Fonte { get; init; }
    public string Titulo { get; init; }
    public string Preco { get; init; }
    public string PrecoAnterior { get; init; }
    public string Link { get; init; }
    public string ImagemUrl { get; init; }
    public int? PorcentagemComissao { get; init; }
}