namespace VoyLlegando.Application.DTOs;

public class CampoResponse
{
    public int IdCampo { get; set; }

    public int IdProductor { get; set; }

    public string DescripCampo { get; set; } = "";

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }
}