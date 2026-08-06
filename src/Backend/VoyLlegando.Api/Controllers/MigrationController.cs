using Microsoft.AspNetCore.Mvc;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Infrastructure.Security;

namespace VoyLlegando.Api.Controllers;

[ApiController]
[Route("api/migration")]
public class MigrationController : ControllerBase
{
    private readonly IUsuarioRepository _repo;
    private readonly IPasswordService _password;

    public MigrationController(
        IUsuarioRepository repo,
        IPasswordService password)
    {
        _repo = repo;
        _password = password;
    }

    [HttpPost("bcrypt")]
    public async Task<IActionResult> Migrar()
    {
        var usuarios = await _repo.ObtenerTodosAsync();

        int cantidad = 0;

        foreach (var u in usuarios)
        {
            // Si ya parece un hash BCrypt, lo salteamos
            if (!string.IsNullOrWhiteSpace(u.Clave) &&
                (u.Clave.StartsWith("$2a$") ||
                 u.Clave.StartsWith("$2b$") ||
                 u.Clave.StartsWith("$2y$")))
                continue;

            var hash = _password.GenerarHash(u.Clave);
	    Console.WriteLine($"Id={u.IdUsuario} Cel={u.Celular} Clave={u.Clave}");
            await _repo.ActualizarClaveAsync(
                u.IdUsuario,
                hash);

            cantidad++;
        }

        return Ok(new
        {
            mensaje = "Migración terminada",
            usuariosActualizados = cantidad
        });
    }
}