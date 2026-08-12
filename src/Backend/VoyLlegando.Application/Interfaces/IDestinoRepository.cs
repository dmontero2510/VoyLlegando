using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Application.Interfaces;

public interface IDestinoRepository
{
    Task<IEnumerable<Destino>> ObtenerPorPlantaAsync(
        int idPlanta);

    Task<Destino?> ObtenerPorIdAsync(
        int idDestino);

    Task<int> CrearAsync(
        Destino destino);

    Task ActualizarAsync(
        Destino destino);

    Task EliminarAsync(
        int idDestino);
}