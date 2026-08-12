namespace VoyLlegando.Application.DTOs;

public class UsuarioResponse
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Domicilio { get; set; } = string.Empty;

    public string Iva { get; set; } = string.Empty;

    public string Cuit { get; set; } = string.Empty;

    public string Celular { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;

    public bool Habilitado { get; set; }

    public int? IdTranspor { get; set; }

    public int? IdPlanta { get; set; }

    public int? IdProduc { get; set; }

    public string? PatChasis { get; set; }

    public string? PatAcopla { get; set; }

    public bool? Batea { get; set; }

    public bool? Corta { get; set; }

    public bool? Larga { get; set; }

    public bool? Escala { get; set; }

    public string? Estado { get; set; }
}