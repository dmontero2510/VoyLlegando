using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Application.Interfaces;

public interface ICerealRepository
{
    Task<IEnumerable<Cereal>> ObtenerTodosAsync();

    Task<IEnumerable<Cereal>> ObtenerHabilitadosAsync();

    Task<Cereal?> ObtenerPorIdAsync(
        int idCereal);

    Task CrearAsync(
        Cereal cereal);

    Task ActualizarAsync(
        Cereal cereal);

    Task BajaAsync(
        int idCereal);
}