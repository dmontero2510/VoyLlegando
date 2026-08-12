namespace VoyLlegando.Application.DTOs;

public class ProductorRequest
{
    public string Nombre { get; set; } = "";

    public string? Domicilio { get; set; }

    public string? Iva { get; set; }

    public string Cuit { get; set; } = "";

    public bool Habilitado { get; set; } = true;
}