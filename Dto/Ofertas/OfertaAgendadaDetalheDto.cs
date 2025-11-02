namespace plataforma.ofertas.Dto.Ofertas;

public class OfertaAgendadaDetalheDto
{
    public Guid Id { get; set; }
    public string Fonte { get; set; }
    public string Titulo { get; set; }
    public string PrecoAtual { get; set; }
    public string PrecoAnterior { get; set; }
    public string Link { get; set; }
    public string ImagemUrl { get; set; }
    public DateTime DataHoraAgendamento { get; set; }
    public string Status { get; set; }
    public Guid CtaId { get; set; }
    public Guid TemplateId { get; set; }
    public string Cupom { get; set; }
    public bool TemCupom { get; set; }
    public int? PorcentagemComissao { get; set; }
    public int? ValorComissao { get; set; }
}