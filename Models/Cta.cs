using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace plataforma.ofertas.Models;

[Table("ctas")]
public class Cta : BaseModel
{
    [PrimaryKey("id")] public Guid Id { get; set; }
    
    [Column("titulo")] public string Titulo { get; set; }
}