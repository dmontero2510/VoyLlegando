namespace VoyLlegando.Application.DTOs;

public class RecorridoResponse
{
    public int IdViaje { get; set; }

    public PuntoRecorrido Origen { get; set; } = new();

    public PuntoRecorrido Destino { get; set; } = new();
}

public class PuntoRecorrido
{
    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }
}