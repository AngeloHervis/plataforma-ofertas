namespace plataforma.ofertas.Dto.Ofertas;

public class OfertaAgendadaDetalheDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string Preco { get; set; }
    public string Link { get; set; }
    public string ImagemUrl { get; set; }
    public string Descricao { get; set; }
    public DateTime DataHoraEnvio { get; set; }
    public bool Enviada { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}