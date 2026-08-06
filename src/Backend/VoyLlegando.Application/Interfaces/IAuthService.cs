using VoyLlegando.Application.DTOs;

namespace VoyLlegando.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> Login(LoginRequest request);
}