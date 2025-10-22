using Supabase;

namespace plataforma.ofertas.Storage;

public class SupabaseContext
{
    public Client Client { get; }

    public SupabaseContext(IConfiguration configuration)
    {
        // var url = configuration["Supabase:Url"];
        // var key = configuration["Supabase:AnonKey"];
        var url = "https://rnfhdjntuanbttjmjvkx.supabase.co";
        var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InJuZmhkam50dWFuYnR0am1qdmt4Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjA5ODc5ODAsImV4cCI6MjA3NjU2Mzk4MH0.DhUb4qS2cgG-z-oiKxwsAobVDuOTTvQFE5D7Jg3H3jw";

        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(key))
            throw new InvalidOperationException("Configurações do Supabase ausentes.");

        var options = new SupabaseOptions { AutoConnectRealtime = false };
        Client = new Client(url, key, options);
    }
}