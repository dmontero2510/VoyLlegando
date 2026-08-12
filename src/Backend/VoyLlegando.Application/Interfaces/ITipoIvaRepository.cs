using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Application.Interfaces;

public interface ITipoIvaRepository
{
    Task<IEnumerable<TipoIva>> ObtenerTodosAsync();
}