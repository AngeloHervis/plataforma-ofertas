using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace plataforma.ofertas.Models;

[Table("ofertas_agendadas")]
public class OfertaAgendada : BaseModel
{
    [PrimaryKey("id", false)] public Guid Id { get; set; }
    [Column("titulo")] public string Titulo { get; set; }
    [Column("preco_atual")] public string PrecoAtual { get; set; }
    [Column("preco_anterior")] public string PrecoAnterior { get; set; }
    [Column("link")] public string Link { get; set; }
    [Column("imagem_url")] public string ImagemUrl { get; set; }
    [Column("data_hora_envio")] public DateTime DataHoraEnvio { get; set; }
    [Column("status")] public string Status { get; set; }
    [Column("criado_em")] public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    [Column("atualizado_em")] public DateTime? AtualizadoEm { get; set; }
    [Column("fonte")] public string Fonte { get; set; }
    [Column("porcentagem_comissao")] public int? PorcentagemComissao { get; set; }
    [Column("cta")] public string Cta { get; set; }
    [Column("template_id")] public Guid TemplateId { get; set; }
    [Column("cupom")] public string Cupom { get; set; }
    [Column("tem_cupom")] public bool TemCupom { get; set; }
    [Column("imagem_url_whatsapp")] public string ImagemUrlWhatsapp { get; set; }
    [Column("imagem_url_site")] public string ImagemUrlSite { get; set; }
    [Column("descricao")] public string Descricao { get; set; }

}