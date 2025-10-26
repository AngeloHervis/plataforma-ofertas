namespace plataforma.ofertas.Models;

public class ProductInfo
{
    public string Titulo { get; set; } = "Não encontrado";
    public string Link { get; set; } = "Não encontrado";
    public string ImagemUrl { get; set; } = "Não encontrado";
    public string PrecoAtual { get; set; } = "Não encontrado";
    public string PrecoAnterior { get; set; } = "Não encontrado";

    public bool IsValid => !string.IsNullOrWhiteSpace(Titulo) &&
                           !string.IsNullOrWhiteSpace(Link) &&
                           !string.IsNullOrWhiteSpace(PrecoAtual);
}
