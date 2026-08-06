using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Application.Interfaces;

public interface IJwtService
{
    string GenerarToken(Usuario usuario);
}