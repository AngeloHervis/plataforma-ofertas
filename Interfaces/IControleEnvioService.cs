using plataforma.ofertas.Models;

namespace plataforma.ofertas.Interfaces;

public interface IControleEnvioService
{
    Task<List<Oferta>> FiltrarNovasAsync(List<Oferta> ofertas);
}