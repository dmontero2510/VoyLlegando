namespace VoyLlegando.Domain.Entities;

public class ViajeDocumento
{
    public int IdDocumento { get; set; }

    public int IdViaje { get; set; }

    public string Tipo { get; set; } = "";

    public string? NombreArchivo { get; set; }

    public byte[] Contenido { get; set; } = Array.Empty<byte>();

    public DateTime Fecha { get; set; }
}