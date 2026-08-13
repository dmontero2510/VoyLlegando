using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Application.Interfaces;

public interface ILogisticaRepository
{
    Task<Logistica?> ObtenerPorIdAsync(
        int idTranspor);

    Task<IEnumerable<Logistica>>
        ObtenerTodosAsync();

    Task<int> CrearAsync(
        Logistica logistica);
}
