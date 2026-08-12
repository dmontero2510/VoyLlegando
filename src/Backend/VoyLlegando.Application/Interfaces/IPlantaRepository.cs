using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Application.Interfaces;

public interface IPlantaRepository
{
    Task<IEnumerable<Planta>> ObtenerTodosAsync(
        int idTranspor);

    Task<Planta?> ObtenerPorIdAsync(
        int idPlanta,
        int idTranspor);

    Task<int> CrearAsync(
        Planta planta);

    Task ActualizarAsync(
        Planta planta);

    Task BajaAsync(
        int idPlanta,
        int idTranspor);
}