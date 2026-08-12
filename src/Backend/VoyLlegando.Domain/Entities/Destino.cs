namespace VoyLlegando.Domain.Entities;

public class Destino
{
    public int IdDestino { get; set; }

    public int IdPlanta { get; set; }

    public string DescripDestino { get; set; } = "";

    public DateTime? FechaVinculacion { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }
}