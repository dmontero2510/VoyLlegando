namespace VoyLlegando.Application.DTO;

public class LoginResponse
{
    public string Token { get; set; } = "";

    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = "";

    public string Rol { get; set; } = "";

    public int? IdTranspor { get; set; }
}