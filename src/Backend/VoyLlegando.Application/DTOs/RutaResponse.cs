namespace VoyLlegando.Application.DTOs;

public class RutaResponse
{
public int IdViaje { get; set; }

public PuntoRuta Origen { get; set; } = new();

public PuntoRuta Destino { get; set; } = new();

public decimal DistanciaKm { get; set; }

public decimal DuracionMinutos { get; set; }

public bool EsAproximada { get; set; }

public string Aviso { get; set; } = "";

public List<PuntoRuta> Ruta { get; set; } = new();

}

public class PuntoRuta
{
public double Latitud { get; set; }

public double Longitud { get; set; }

}
