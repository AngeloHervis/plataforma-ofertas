namespace plataforma.ofertas.Dto.Agendamentos;

public class AtualizarEnvioDto
{
    public string Titulo { get; set; }
    public string Preco { get; set; }
    public string Link { get; set; }
    public string ImagemUrl { get; set; }
    public string Descricao { get; set; }
    public DateTime DataHoraEnvio { get; set; }
    public bool Enviada { get; set; }
}