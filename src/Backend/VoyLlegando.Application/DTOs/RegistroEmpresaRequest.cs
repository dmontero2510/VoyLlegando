namespace VoyLlegando.Application.DTOs;

public class RegistroEmpresaRequest
{
    public string Celular { get; set; } = "";

    public string Clave { get; set; } = "";

    public string Nombre { get; set; } = "";

    public string? Domicilio { get; set; }

    public string Iva { get; set; } = "1";

    public string Cuit { get; set; } = "";

    public string Email { get; set; } = "";

    public string? PatChasis { get; set; }

    public string? PatAcopla { get; set; }

    public bool Batea { get; set; }

    public bool Corta { get; set; }

    public bool Larga { get; set; }

    public bool Escala { get; set; }
}