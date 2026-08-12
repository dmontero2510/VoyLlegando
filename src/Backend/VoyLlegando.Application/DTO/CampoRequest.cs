namespace VoyLlegando.Application.DTOs;

public class CampoRequest
{
    public int IdProductor { get; set; }

    public string DescripCampo { get; set; } = "";

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }
}