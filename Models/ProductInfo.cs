namespace plataforma.ofertas.Models;

public class ProductInfo
{
    public string Titulo { get; set; }
    public string Link { get; set; }
    public string ImagemUrl { get; set; }
    public string PrecoAtual { get; set; }
    public string PrecoAnterior { get; set; }

    public bool IsValid => !string.IsNullOrWhiteSpace(Titulo) &&
                           !string.IsNullOrWhiteSpace(Link) &&
                           !string.IsNullOrWhiteSpace(PrecoAtual);
}
