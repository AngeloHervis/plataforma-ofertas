namespace plataforma.ofertas.Dto.Ofertas;

public class AgendarOfertaRequestDto
{
    public Guid OfertaId { get; set; }
    public CamposEditados CamposEditados { get; set; }
    public DateTime DataHoraAgendamento { get; set; }
}

public class CamposEditados
{
    public string Titulo { get; set; }
    public string Preco { get; set; }
    public string Link { get; set; }
    public string ImagemUrl { get; set; }
    public string Descricao { get; set; }
}