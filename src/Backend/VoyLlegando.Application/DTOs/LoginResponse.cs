namespace VoyLlegando.Application.DTOs;

public class LoginResponse
{
    public bool Success { get; set; }

    public string Token { get; set; } = string.Empty;

    public string Mensaje { get; set; } = string.Empty;

    public PerfilResponse? Usuario { get; set; }
}