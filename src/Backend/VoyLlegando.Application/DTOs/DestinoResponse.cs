namespace VoyLlegando.Application.DTOs;

public class DestinoResponse
{
    public int IdDestino { get; set; }

    public int IdPlanta { get; set; }

    public string DescripDestino { get; set; } = "";

    public DateTime? FechaVinculacion { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }
}