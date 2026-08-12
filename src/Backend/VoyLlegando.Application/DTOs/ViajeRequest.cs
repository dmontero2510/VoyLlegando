namespace VoyLlegando.Application.DTOs;

public class ViajeRequest
{
public int? IdCamionero { get; set; }

public string Tipo { get; set; } = "";

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

public string Observaciones { get; set; } = "";

public bool? Batea { get; set; }

public bool? Corta { get; set; }

public bool? Larga { get; set; }

}
