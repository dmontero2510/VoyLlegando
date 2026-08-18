namespace VoyLlegando.Domain.Entities;

public class Cereal
{
    public int IdCereal { get; set; }

    public string NombreCereal { get; set; } = "";

    public string Categoria { get; set; } = "";

    public bool Habilitado { get; set; }
}
