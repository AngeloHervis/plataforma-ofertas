namespace plataforma.ofertas.Interfaces.Jobs;

public interface IRunnableService
{
    string Name { get; } 
    Task RunAsync(CancellationToken ct);
}