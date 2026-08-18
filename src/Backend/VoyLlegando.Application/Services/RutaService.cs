using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using VoyLlegando.Application.DTOs;

namespace VoyLlegando.Application.Services;

public class RutaService
{
private readonly HttpClient _httpClient;

public RutaService(HttpClient httpClient)
{
    _httpClient = httpClient;
}

public async Task<RutaResponse> CalcularAsync(
    int idViaje,
    decimal latitudOrigen,
    decimal longitudOrigen,
    decimal latitudDestino,
    decimal longitudDestino)
{
    // OSRM utiliza el orden:
    // longitud,latitud
    // NO latitud,longitud.

    var lonOrigen = longitudOrigen.ToString(
        CultureInfo.InvariantCulture);

    var latOrigen = latitudOrigen.ToString(
        CultureInfo.InvariantCulture);

    var lonDestino = longitudDestino.ToString(
        CultureInfo.InvariantCulture);

    var latDestino = latitudDestino.ToString(
        CultureInfo.InvariantCulture);

    var url =
        $"route/v1/driving/" +
        $"{lonOrigen},{latOrigen};" +
        $"{lonDestino},{latDestino}" +
        "?overview=full&geometries=geojson";

    OsrmResponse? respuesta;

    try
    {
        respuesta =
            await _httpClient.GetFromJsonAsync<OsrmResponse>(url);
    }
    catch (HttpRequestException)
    {
        return CrearRecorridoAproximado(
            idViaje,
            latitudOrigen,
            longitudOrigen,
            latitudDestino,
            longitudDestino);
    }
    catch (TaskCanceledException)
    {
        return CrearRecorridoAproximado(
            idViaje,
            latitudOrigen,
            longitudOrigen,
            latitudDestino,
            longitudDestino);
    }

    if (respuesta == null ||
        respuesta.Code != "Ok" ||
        respuesta.Routes.Count == 0)
    {
        return CrearRecorridoAproximado(
            idViaje,
            latitudOrigen,
            longitudOrigen,
            latitudDestino,
            longitudDestino);
    }

    var ruta = respuesta.Routes[0];

    var puntos = ruta.Geometry.Coordinates
        .Select(c => new PuntoRuta
        {
            // GeoJSON también viene:
            // longitud, latitud
            Longitud = c[0],
            Latitud = c[1]
        })
        .ToList();

    return new RutaResponse
    {
        IdViaje = idViaje,

        Origen = new PuntoRuta
        {
            Latitud = (double)latitudOrigen,
            Longitud = (double)longitudOrigen
        },

        Destino = new PuntoRuta
        {
            Latitud = (double)latitudDestino,
            Longitud = (double)longitudDestino
        },

        DistanciaKm =
            Math.Round((decimal)ruta.Distance / 1000m, 2),

        DuracionMinutos =
            Math.Round((decimal)ruta.Duration / 60m, 1),

        EsAproximada = false,

        Ruta = puntos
    };
}

private static RutaResponse CrearRecorridoAproximado(
    int idViaje,
    decimal latitudOrigen,
    decimal longitudOrigen,
    decimal latitudDestino,
    decimal longitudDestino)
{
    var distanciaKm =
        CalcularDistanciaDirectaKm(
            (double)latitudOrigen,
            (double)longitudOrigen,
            (double)latitudDestino,
            (double)longitudDestino);

    return new RutaResponse
    {
        IdViaje = idViaje,

        Origen = new PuntoRuta
        {
            Latitud = (double)latitudOrigen,
            Longitud = (double)longitudOrigen
        },

        Destino = new PuntoRuta
        {
            Latitud = (double)latitudDestino,
            Longitud = (double)longitudDestino
        },

        DistanciaKm =
            Math.Round((decimal)distanciaKm, 2),

        DuracionMinutos = 0,

        EsAproximada = true,

        Aviso =
            "Servicio de rutas no disponible. Se muestra una línea directa y una distancia aproximada.",

        Ruta = new List<PuntoRuta>
        {
            new()
            {
                Latitud = (double)latitudOrigen,
                Longitud = (double)longitudOrigen
            },
            new()
            {
                Latitud = (double)latitudDestino,
                Longitud = (double)longitudDestino
            }
        }
    };
}

private static double CalcularDistanciaDirectaKm(
    double latitudOrigen,
    double longitudOrigen,
    double latitudDestino,
    double longitudDestino)
{
    const double radioTierraKm = 6371.0088;

    var lat1 = GradosARadianes(latitudOrigen);
    var lat2 = GradosARadianes(latitudDestino);
    var diferenciaLatitud =
        GradosARadianes(latitudDestino - latitudOrigen);
    var diferenciaLongitud =
        GradosARadianes(longitudDestino - longitudOrigen);

    var a =
        Math.Sin(diferenciaLatitud / 2) *
        Math.Sin(diferenciaLatitud / 2) +
        Math.Cos(lat1) *
        Math.Cos(lat2) *
        Math.Sin(diferenciaLongitud / 2) *
        Math.Sin(diferenciaLongitud / 2);

    var c =
        2 * Math.Atan2(
            Math.Sqrt(a),
            Math.Sqrt(1 - a));

    return radioTierraKm * c;
}

private static double GradosARadianes(double grados)
{
    return grados * Math.PI / 180;
}

// -------------------------------------------------------
// DTOs INTERNOS DE OSRM
// -------------------------------------------------------

private class OsrmResponse
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = "";

    [JsonPropertyName("routes")]
    public List<OsrmRoute> Routes { get; set; } = new();
}

private class OsrmRoute
{
    [JsonPropertyName("distance")]
    public double Distance { get; set; }

    [JsonPropertyName("duration")]
    public double Duration { get; set; }

    [JsonPropertyName("geometry")]
    public OsrmGeometry Geometry { get; set; } = new();
}

private class OsrmGeometry
{
    [JsonPropertyName("coordinates")]
    public List<double[]> Coordinates { get; set; } = new();
}
}
