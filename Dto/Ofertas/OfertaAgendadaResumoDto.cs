namespace plataforma.ofertas.Dto.Ofertas;

public class OfertaAgendadaResumoDto
{
    public Guid OfertaId { get; set; }
    public string Titulo { get; set; }
    public string PrecoAtual { get; set; }
    public string PrecoAnterior { get; set; }
    public string Link { get; set; }
    public int? PorcentagemComissao { get; set; }
    public string ImagemUrl { get; set; }
    public DateTime DataHoraAgendamento { get; set; }
    public string Status { get; set; }
}