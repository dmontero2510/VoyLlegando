namespace VoyLlegando.Domain.Entities;

public class Planta
{
    public int IdPlanta { get; set; }

    public int IdTranspor { get; set; }

    public string Nombre { get; set; } = "";

    public string? Domicilio { get; set; }

    public string? Iva { get; set; }

    public string Cuit { get; set; } = "";

    public bool Habilitado { get; set; }
}