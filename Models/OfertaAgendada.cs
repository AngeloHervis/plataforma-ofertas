using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace plataforma.ofertas.Models;

[Table("ofertas_agendadas")]
public class OfertaAgendada : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("titulo")]
    public string Titulo { get; set; }

    [Column("preco_atual")]
    public string PrecoAtual { get; set; }
    
    [Column("preco_anterior")]
    public string PrecoAnterior { get; set; }

    [Column("link")]
    public string Link { get; set; }

    [Column("imagem_url")]
    public string ImagemUrl { get; set; }

    [Column("data_hora_envio")]
    public DateTime DataHoraEnvio { get; set; }

    [Column("enviada")]
    public bool Enviada { get; set; } = false;

    [Column("criado_em")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    [Column("atualizado_em")]
    public DateTime? AtualizadoEm { get; set; }
    
    [Column("cta")]
    public string Cta { get; set; }
    
    [Column("porcentagem_comissao")]
    public int? PorcentagemComissao { get; set; }
}