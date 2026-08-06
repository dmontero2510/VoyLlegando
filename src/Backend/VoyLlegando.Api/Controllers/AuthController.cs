using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoyLlegando.Application.DTOs;
using VoyLlegando.Application.Interfaces;

using UsuarioRepositoryInterface = VoyLlegando.Application.Interfaces.IUsuarioRepository;

namespace VoyLlegando.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly UsuarioRepositoryInterface _usuarioRepository;


    public AuthController(
        IAuthService authService,
        UsuarioRepositoryInterface usuarioRepository)
    {
        _authService = authService;
        _usuarioRepository = usuarioRepository;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.Login(request);

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }


    [Authorize]
    [HttpGet("perfil")]
    public async Task<IActionResult> Perfil()
    {
        var idUsuario = User.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;


        if (idUsuario == null)
            return Unauthorized();


        var usuario = await _usuarioRepository
            .ObtenerPorIdAsync(int.Parse(idUsuario));


        if (usuario == null)
            return NotFound();


        return Ok(new
        {
            id = usuario.IdUsuario,
            nombre = usuario.Nombre,
            celular = usuario.Celular,
            rol = usuario.Rol,
            idTranspor = usuario.IdTranspor,
            patChasis = usuario.PatChasis,
            patAcopla = usuario.PatAcopla,
            batea = usuario.Batea,
            corta = usuario.Corta,
            larga = usuario.Larga,
            escala = usuario.Escala,
            estado = usuario.Estado
        });
    }
}