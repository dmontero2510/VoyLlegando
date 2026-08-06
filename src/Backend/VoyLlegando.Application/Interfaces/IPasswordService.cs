namespace VoyLlegando.Application.Interfaces;

public interface IPasswordService
{
    string GenerarHash(string password);

    bool Verificar(string password, string hash);
}