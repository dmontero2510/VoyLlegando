using BCrypt.Net;
using VoyLlegando.Application.Interfaces;

namespace VoyLlegando.Infrastructure.Security;

public class PasswordService : IPasswordService
{
    public string GenerarHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }


    public bool Verificar(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
