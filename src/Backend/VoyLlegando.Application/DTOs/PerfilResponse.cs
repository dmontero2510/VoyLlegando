namespace VoyLlegando.Application.DTOs;

public class PerfilResponse
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;

    public bool DebeCambiarClave { get; set; }

    public string? PatChasis { get; set; }

    public string? PatAcoplado { get; set; }

    public bool Batea { get; set; }

    public bool Corta { get; set; }

    public bool Larga { get; set; }

    public bool Escala { get; set; }

    public string Estado { get; set; } = string.Empty;
}
