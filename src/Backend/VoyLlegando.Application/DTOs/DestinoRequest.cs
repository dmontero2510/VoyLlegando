namespace VoyLlegando.Application.DTOs;

public class DestinoRequest
{
    public int IdPlanta { get; set; }

    public string DescripDestino { get; set; } = "";

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }
}