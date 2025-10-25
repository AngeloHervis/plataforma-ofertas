namespace plataforma.ofertas.Dto.Ofertas;

public class AgendarOfertaRequestDto
{
    public CamposEditados CamposEditados { get; set; }
    public DateTime DataHoraAgendamento { get; set; }
}

public class CamposEditados
{
    public string Titulo { get; set; }
    public string PrecoAtual { get; set; }
    public string PrecoAnterior { get; set; }
    public string Link { get; set; }
    public string ImagemUrl { get; set; }
}