namespace plataforma.ofertas.Dto.Ofertas;

public record OfertaDetalheDto 
{
    public Guid Id { get; init; }
    public string Fonte { get; init; }
    public string Titulo { get; init; }
    public string Preco { get; init; }
    public string Link { get; init; }
    public string ImagemUrl { get; init; }
    public string Descricao { get; init; }
    public DateTime PublicadoEm { get; init; }
}