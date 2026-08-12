namespace VoyLlegando.Domain.Entities;

public class Campo
{
    public int IdCampo { get; set; }

    public int IdProductor { get; set; }

    public string DescripCampo { get; set; } = "";

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }
}