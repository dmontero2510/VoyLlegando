using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Application.Interfaces;

public interface IViajeDocumentoRepository
{
    Task<int> CrearAsync(ViajeDocumento documento);

    Task<IEnumerable<ViajeDocumento>>
        ObtenerPorViajeAsync(int idViaje);

    Task<ViajeDocumento?>
        ObtenerPorIdAsync(int idDocumento);

    Task EliminarAsync(int idDocumento);
}