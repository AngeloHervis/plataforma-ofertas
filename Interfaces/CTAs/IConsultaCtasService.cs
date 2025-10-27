using plataforma.ofertas._Base;

namespace plataforma.ofertas.Interfaces.CTAs;

public interface IConsultaCtasService
{
    Task<CommandResult<List<string>>> ConsultarAsync(CancellationToken cancellationToken);
}