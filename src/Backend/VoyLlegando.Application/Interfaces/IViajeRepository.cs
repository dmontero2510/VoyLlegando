using VoyLlegando.Application.DTOs;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Application.Interfaces;

public interface IViajeRepository
{
    Task<IEnumerable<Viaje>>
        ObtenerTodosAsync();

    Task<IEnumerable<Viaje>>
        ObtenerPorTransporAsync(
            int idTranspor);

    Task<IEnumerable<Viaje>>
        ObtenerPorCamioneroAsync(
            int idCamionero);

    Task<Viaje?>
        ObtenerPorIdAsync(
            int idViaje);

    Task<int>
        CrearAsync(
            Viaje viaje);

    Task ActualizarAsync(
        Viaje viaje);

    Task<IEnumerable<ViajePendienteResponse>>
        ObtenerPendientesParaEmpresaAsync(
            int idUsuario);

    Task TomarPendienteAsync(
        int idViaje,
        int idEmpresa);
}