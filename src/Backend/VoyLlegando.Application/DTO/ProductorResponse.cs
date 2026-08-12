namespace VoyLlegando.Application.DTOs;

public class ProductorResponse
{
    public int IdProductor { get; set; }

    public string Nombre { get; set; } = "";

    public string? Domicilio { get; set; }

    public string? Iva { get; set; }

    public string Cuit { get; set; } = "";

    public bool Habilitado { get; set; }
}