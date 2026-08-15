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

    Task<IEnumerable<Usuario>>
        ObtenerSolicitudesPendientesAsync(
            int idTranspor);

    Task AceptarSolicitudAsync(
        int idTranspor,
        int idUsuario);

    Task RechazarSolicitudAsync(
        int idTranspor,
        int idUsuario);

    Task<IEnumerable<Usuario>>
        ObtenerEmpresasAceptadasDisponiblesAsync(
            int idTranspor);

    Task<IEnumerable<LogisticaCamionRelacion>>
        ObtenerRelacionesAsync(
            int idTranspor);

    Task<bool> BloquearRelacionAsync(
        int idTranspor,
        int idUsuario);

    Task<bool> RehabilitarRelacionAsync(
        int idTranspor,
        int idUsuario);
}

public class LogisticaCamionRelacion
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = "";

    public string? Cuit { get; set; }

    public string? Celular { get; set; }

    public string? Email { get; set; }

    public bool Habilitado { get; set; }

    public string? EstadoEmpresa { get; set; }

    public string EstadoRelacion { get; set; } = "";

    public string DescripcionEstado { get; set; } = "";

    public DateTime? FechaVinculacion { get; set; }
}
