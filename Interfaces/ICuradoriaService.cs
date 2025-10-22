using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces;

public interface ICuradoriaService
{
    Task<List<Oferta>> FiltrarAsync(List<Oferta> ofertas);
}