namespace VoyLlegando.Application.DTOs;

public class ViajePendienteResponse
{
    public int IdViaje { get; set; }

    public int IdTranspor { get; set; }

    public string Logistica { get; set; } = "";

    public int IdProduc { get; set; }

    public string Productor { get; set; } = "";

    public int IdOrigen { get; set; }

    public string Origen { get; set; } = "";

    public int IdPlanta { get; set; }

    public string Planta { get; set; } = "";

    public int IdDestino { get; set; }

    public string Destino { get; set; } = "";

    public int IdCereal { get; set; }

    public string Cereal { get; set; } = "";

    public DateTime? FechaPedido { get; set; }

    public string Ctg { get; set; } = "";

    public decimal Kms { get; set; }

    public decimal Tarifa { get; set; }

    public string Estado { get; set; } = "";

    public string DescripVia { get; set; } = "";

    public string Observaciones { get; set; } = "";

    public bool? Batea { get; set; }

    public bool? Corta { get; set; }

    public bool? Larga { get; set; }
}