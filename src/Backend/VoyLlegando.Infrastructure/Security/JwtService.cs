using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Infrastructure.Security;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // -------------------------------------------------------
    // GENERAR TOKEN JWT
    // -------------------------------------------------------

    public string GenerarToken(Usuario usuario)
    {
        var keyValue = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(keyValue))
            throw new InvalidOperationException(
                "No está configurada la clave JWT.");

        var issuer = _configuration["Jwt:Issuer"];

        if (string.IsNullOrWhiteSpace(issuer))
            throw new InvalidOperationException(
                "No está configurado Jwt:Issuer.");

        var audience = _configuration["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(audience))
            throw new InvalidOperationException(
                "No está configurado Jwt:Audience.");

        var expireMinutesValue =
            _configuration["Jwt:ExpireMinutes"];

        if (!double.TryParse(
                expireMinutesValue,
                out var expireMinutes))
        {
            throw new InvalidOperationException(
                "Jwt:ExpireMinutes no tiene un valor válido.");
        }

        // ---------------------------------------------------
        // CLAVE DE FIRMA
        // ---------------------------------------------------

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(keyValue));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        // ---------------------------------------------------
        // CLAIMS
        // ---------------------------------------------------

        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                usuario.IdUsuario.ToString()),

            new Claim(
                ClaimTypes.Name,
                usuario.Nombre),

            new Claim(
                ClaimTypes.Role,
                usuario.Rol),

            new Claim(
                "celular",
                usuario.Celular)
        };

        // ---------------------------------------------------
        // TOKEN
        // ---------------------------------------------------

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}