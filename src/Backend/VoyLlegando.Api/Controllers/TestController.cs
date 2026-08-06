using Microsoft.AspNetCore.Mvc;
using VoyLlegando.Application.Interfaces;

namespace VoyLlegando.Api.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;

    public TestController(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    [HttpGet("usuario/{celular}")]
    public async Task<IActionResult> BuscarUsuario(string celular)
    {
        var usuario = await _usuarioRepository.ObtenerPorCelularAsync(celular);
        if (usuario == null)
            return NotFound("Usuario no encontrado.");

        return Ok(new
        {
            usuario.IdUsuario,
            usuario.Nombre,
            usuario.Celular,
            usuario.Rol,
            usuario.Habilitado,
            usuario.IdTranspor
        });
    }
}