namespace VoyLlegando.Domain.Entities;

public class Viaje
{
    public int IdViaje { get; set; }

    public int IdTranspor { get; set; }

    public int? IdCamionero { get; set; }

    public string Tipo { get; set; } = "";

    public DateTime? FechaPedido { get; set; }

    public int IdCereal { get; set; }

    public int IdProduc { get; set; }

    public int IdOrigen { get; set; }

    public int IdPlanta { get; set; }

    public int IdDestino { get; set; }

    public string Origen { get; set; } = "";

    public string Destino { get; set; } = "";

    public string Ctg { get; set; } = "";

    public decimal Kms { get; set; }

    public decimal Tarifa { get; set; }

    public string Estado { get; set; } = "P";

    public DateTime? FechaAsigna { get; set; }

    public DateTime? FechaTermina { get; set; }

    public string Observaciones { get; set; } = "";

    public bool? Batea { get; set; }

    public bool? Corta { get; set; }

    public bool? Larga { get; set; }

    public int IdUsuario { get; set; }

    public decimal? LatitudOrigen { get; set; }

    public decimal? LongitudOrigen { get; set; }

    public decimal? LatitudDestino { get; set; }

    public decimal? LongitudDestino { get; set; }
}
