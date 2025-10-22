using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace plataforma.ofertas.Models;

[Table("ofertas")]
public class Oferta : BaseModel
{
    [PrimaryKey("id")] public Guid Id { get; set; }

    [Column("fonte")] public string Fonte { get; set; }

    [Column("titulo")] public string Titulo { get; set; }

    [Column("preco_atual")] public string PrecoAtual { get; set; }

    [Column("preco_anterior")] public string PrecoAnterior { get; set; }

    [Column("desconto_percentual")] public int? DescontoPercentual { get; set; }

    [Column("link")] public string Link { get; set; } = string.Empty;

    [Column("imagem_url")] public string ImagemUrl { get; set; }

    [Column("publicado_em")] public DateTime PublicadoEm { get; set; } = DateTime.UtcNow;
}