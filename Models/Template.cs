using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace plataforma.ofertas.Models;

[Table("templates")]
public class Template : BaseModel
{
    [PrimaryKey("id")] public Guid Id { get; set; }
    [Column("conteudo")] public string Conteudo { get; set; }
    [Column("nome")] public string Nome { get; set; }
    [Column("descricao")] public string Descricao { get; set; }
}