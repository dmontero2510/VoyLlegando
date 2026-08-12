namespace VoyLlegando.Domain.Entities;

public class ViajeEvento
{
    public int IdEvento { get; set; }

    public int IdViaje { get; set; }

    public string? EstadoAnterior { get; set; }

    public string EstadoNuevo { get; set; } = "";

    public DateTime Fecha { get; set; }

    public int IdUsuario { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }

    public string? Observaciones { get; set; }
}