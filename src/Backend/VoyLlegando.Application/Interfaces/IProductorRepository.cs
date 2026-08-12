using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Application.Interfaces;

public interface IProductorRepository
{
    Task<IEnumerable<Productor>> ObtenerTodosAsync(
        int idTranspor);

    Task<Productor?> ObtenerPorIdAsync(
        int idProductor,
        int idTranspor);

    Task<int> CrearAsync(
        Productor productor);

    Task ActualizarAsync(
        Productor productor);

    Task BajaAsync(
        int idProductor,
        int idTranspor);
}