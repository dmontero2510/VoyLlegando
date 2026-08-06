namespace VoyLlegando.Application.DTOs;

public class UsuarioRequest
{
    public string Celular { get; set; } = "";
    public string Clave { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string? Domicilio { get; set; }
    public string Iva { get; set; } ="1";
    public string Cuit { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = "";

    public int? IdTranspor { get; set; }
    public int? IdPlanta { get; set; }
    public int? IdProduc { get; set; }

    public string? PatChasis { get; set; }
    public string? PatAcopla { get; set; }

    public bool Batea { get; set; }
    public bool Corta { get; set; } = true;
    public bool Larga { get; set; } = true;
    public bool Escala { get; set; } = true;

    public string Estado { get; set; } = "F";
}