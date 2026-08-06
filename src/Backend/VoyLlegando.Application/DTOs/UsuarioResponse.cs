namespace VoyLlegando.Application.DTOs;

public class UsuarioResponse
{
    public int IdUsuario { get; set; }
    public string Celular { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string? Domicilio { get; set; }
    public int Iva { get; set; }
    public string? Cuit { get; set; }
    public string? Email { get; set; }
    public string Rol { get; set; } = "";
    public bool Habilitado { get; set; }

    public int? IdTranspor { get; set; }
    public int? IdPlanta { get; set; }
    public int? IdProduc { get; set; }

    public string? PatChasis { get; set; }
    public string? PatAcopla { get; set; }

    public bool Batea { get; set; }
    public bool Corta { get; set; }
    public bool Larga { get; set; }
    public bool Escala { get; set; }

    public string? Estado { get; set; }
}