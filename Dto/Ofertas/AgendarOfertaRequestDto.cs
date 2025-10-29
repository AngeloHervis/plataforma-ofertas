namespace plataforma.ofertas.Dto.Ofertas;

public class AgendarOfertaRequestDto
{
    public string Titulo { get; set; }
    public string Fonte { get; set; }
    public string PrecoAtual { get; set; }
    public string PrecoAnterior { get; set; }
    public string Link { get; set; }
    public string ImagemUrl { get; set; }
    public string CtaPersonalizado { get; set; }
    public int? PorcentagemComissao { get; set; }
    public DateTime DataHoraAgendamento { get; set; }
}