namespace VoyLlegando.Application.DTOs;

public class CerealRequest
{
    public int IdCereal { get; set; }

    public string Nombre { get; set; } = "";

    public string Categoria { get; set; } = "";

    public bool Habilitado { get; set; } = true;
}
