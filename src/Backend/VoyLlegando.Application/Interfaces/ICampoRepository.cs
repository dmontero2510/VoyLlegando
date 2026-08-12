using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Application.Interfaces;

public interface ICampoRepository
{
    Task<IEnumerable<Campo>> ObtenerPorProductorAsync(
        int idProductor);

    Task<Campo?> ObtenerPorIdAsync(
        int idCampo);

    Task<int> CrearAsync(
        Campo campo);

    Task ActualizarAsync(
        Campo campo);

    Task EliminarAsync(
        int idCampo);
}