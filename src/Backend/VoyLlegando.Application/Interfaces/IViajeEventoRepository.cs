using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Application.Interfaces;

public interface IViajeEventoRepository
{
    Task<int> CrearAsync(ViajeEvento evento);

    Task<IEnumerable<ViajeEvento>>
        ObtenerPorViajeAsync(int idViaje);
}