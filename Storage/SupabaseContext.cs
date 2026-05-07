using Supabase;

namespace plataforma.ofertas.Storage;

public class SupabaseContext
{
    public Client Client { get; }

    public SupabaseContext(IConfiguration configuration)
    {
        var url = configuration["Supabase:Url"];
        var key = configuration["Supabase:AnonKey"];

        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(key))
            throw new InvalidOperationException("Configurações do Supabase ausentes.");

        var options = new SupabaseOptions { AutoConnectRealtime = false };
        Client = new Client(url, key, options);
    }
}
