using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Application.Interfaces;

public interface ILogisticaCamionRepository
{
    Task<IEnumerable<Logistica>>
        ObtenerVinculadasAsync(
            int idUsuario);

    Task<IEnumerable<Logistica>>
        ObtenerDisponiblesAsync(
            int idUsuario);

    Task VincularAsync(
        int idTranspor,
        int idUsuario);

    Task DesvincularAsync(
        int idTranspor,
        int idUsuario);

    Task<bool> EstaVinculadoAsync(
        int idTranspor,
        int idUsuario);
}