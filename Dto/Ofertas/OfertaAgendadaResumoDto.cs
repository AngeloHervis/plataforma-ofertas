namespace plataforma.ofertas.Dto.Ofertas;

public class OfertaAgendadaResumoDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string Preco { get; set; }
    public string ImagemUrl { get; set; }
    public DateTime DataHoraAgendamento { get; set; }
    public bool Enviada { get; set; }
}