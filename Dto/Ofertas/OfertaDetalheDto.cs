namespace plataforma.ofertas.Dto.Ofertas;

public record OfertaDetalheDto 
{
    public Guid Id { get; set; }
    public string Fonte { get; init; }
    public string Titulo { get; init; }
    public string PrecoAtual { get; init; }
    public string PrecoAnterior { get; init; }
    public string Link { get; init; }
    public string ImagemUrl { get; init; }
    public DateTime? PublicadoEm { get; init; }
}