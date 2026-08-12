using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoyLlegando.Application.DTOs;
using VoyLlegando.Application.Interfaces;

using UsuarioRepositoryInterface =
    VoyLlegando.Application.Interfaces.IUsuarioRepository;

namespace VoyLlegando.Api.Controllers;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService
        _authService;

    private readonly UsuarioRepositoryInterface
        _usuarioRepository;

    private readonly ILogisticaRepository
        _logisticaRepository;

    private readonly IPasswordService
        _passwordService;


    // -------------------------------------------------------
    // CONSTRUCTOR
    // -------------------------------------------------------

    public AuthController(
        IAuthService authService,
        UsuarioRepositoryInterface usuarioRepository,
        ILogisticaRepository logisticaRepository,
        IPasswordService passwordService)
    {
        _authService =
            authService;

        _usuarioRepository =
            usuarioRepository;

        _logisticaRepository =
            logisticaRepository;

        _passwordService =
            passwordService;
    }


    // -------------------------------------------------------
    // POST /api/auth/login
    // -------------------------------------------------------

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var response =
            await _authService.Login(request);


        if (!response.Success)
            return Unauthorized(response);


        return Ok(response);
    }


    // -------------------------------------------------------
    // POST /api/auth/registro
    // -------------------------------------------------------

    [AllowAnonymous]
    [HttpPost("registro")]
    public async Task<IActionResult> Registro(
        RegistroEmpresaRequest request)
    {
        // ---------------------------------------------------
        // CELULAR
        // ---------------------------------------------------

        var celular =
            new string(
                (request.Celular ?? "")
                    .Where(char.IsDigit)
                    .ToArray()
            );


        if (
            string.IsNullOrWhiteSpace(
                celular
            )
        )
        {
            return BadRequest(new
            {
                mensaje =
                    "Ingrese el celular."
            });
        }


        // ---------------------------------------------------
        // CLAVE
        // ---------------------------------------------------

        if (
            string.IsNullOrWhiteSpace(
                request.Clave
            )
        )
        {
            return BadRequest(new
            {
                mensaje =
                    "Ingrese la clave."
            });
        }


        // ---------------------------------------------------
        // NOMBRE
        // ---------------------------------------------------

        if (
            string.IsNullOrWhiteSpace(
                request.Nombre
            )
        )
        {
            return BadRequest(new
            {
                mensaje =
                    "Ingrese el nombre."
            });
        }


        // ---------------------------------------------------
        // CUIT
        // ---------------------------------------------------

        var cuit =
            new string(
                (request.Cuit ?? "")
                    .Where(char.IsDigit)
                    .ToArray()
            );


        if (
            cuit.Length != 11
        )
        {
            return BadRequest(new
            {
                mensaje =
                    "El CUIT debe contener 11 dígitos."
            });
        }


        // ---------------------------------------------------
        // CELULAR REPETIDO
        // ---------------------------------------------------

        var existe =
            await _usuarioRepository
                .ExisteCelularAsync(
                    celular
                );


        if (existe)
        {
            return BadRequest(new
            {
                mensaje =
                    "Ya existe un usuario con ese celular."
            });
        }


        // ---------------------------------------------------
        // CREAR USUARIO
        // ---------------------------------------------------

        var usuario =
            new VoyLlegando.Domain.Entities.Usuario
            {
                Celular =
                    celular,

                Clave =
                    _passwordService
                        .GenerarHash(
                            request.Clave
                        ),

                Nombre =
                    request.Nombre.Trim(),

                Domicilio =
                    string.IsNullOrWhiteSpace(
                        request.Domicilio
                    )
                        ? null
                        : request.Domicilio.Trim(),

                Iva =
                    request.Iva,

                Cuit =
                    cuit,

                Email =
                    request.Email?.Trim()
                    ?? "",


                // -------------------------------------------
                // EL REGISTRO PUBLICO SIEMPRE ES E
                // -------------------------------------------

                Rol =
                    "E",

                Habilitado =
                    true,

                IdTranspor =
                    null,

                IdPlanta =
                    null,

                IdProduc =
                    null,


                // -------------------------------------------
                // DATOS DE TRANSPORTE
                // -------------------------------------------

                PatChasis =
                    string.IsNullOrWhiteSpace(
                        request.PatChasis
                    )
                        ? null
                        : request.PatChasis
                            .Trim()
                            .ToUpperInvariant(),

                PatAcopla =
                    string.IsNullOrWhiteSpace(
                        request.PatAcopla
                    )
                        ? null
                        : request.PatAcopla
                            .Trim()
                            .ToUpperInvariant(),

                Batea =
                    request.Batea,

                Corta =
                    request.Corta,

                Larga =
                    request.Larga,

                Escala =
                    request.Escala,


                // -------------------------------------------
                // NUEVA EMPRESA DISPONIBLE
                // -------------------------------------------

                Estado =
                    "D"
            };


        var id =
            await _usuarioRepository
                .CrearAsync(
                    usuario
                );


        return Ok(new
        {
            ok =
                true,

            idUsuario =
                id,

            mensaje =
                "Registro realizado correctamente."
        });
    }


    // -------------------------------------------------------
    // GET /api/auth/perfil
    // -------------------------------------------------------

    [Authorize]
    [HttpGet("perfil")]
    public async Task<IActionResult> Perfil()
    {
        var idUsuario =
            User.FindFirst(
                ClaimTypes.NameIdentifier)
            ?.Value;


        if (
            !int.TryParse(
                idUsuario,
                out var id
            )
        )
        {
            return Unauthorized();
        }


        var usuario =
            await _usuarioRepository
                .ObtenerPorIdAsync(
                    id
                );


        if (usuario == null)
            return NotFound();


        // ---------------------------------------------------
        // NOMBRE DE LA LOGISTICA
        // ---------------------------------------------------

        string? nombreLogistica =
            null;


        if (
            usuario.Rol == "L" &&
            usuario.IdTranspor.HasValue
        )
        {
            var logistica =
                await _logisticaRepository
                    .ObtenerPorIdAsync(
                        usuario.IdTranspor.Value
                    );


            if (logistica != null)
            {
                nombreLogistica =
                    logistica.Nombre;
            }
        }


        // ---------------------------------------------------
        // RESPUESTA
        // ---------------------------------------------------

        return Ok(new
        {
            id =
                usuario.IdUsuario,

            nombre =
                usuario.Nombre,

            celular =
                usuario.Celular,

            rol =
                usuario.Rol,

            idTranspor =
                usuario.IdTranspor,

            nombreLogistica =
                nombreLogistica,

            patChasis =
                usuario.PatChasis,

            patAcopla =
                usuario.PatAcopla,

            batea =
                usuario.Batea,

            corta =
                usuario.Corta,

            larga =
                usuario.Larga,

            escala =
                usuario.Escala,

            estado =
                usuario.Estado
        });
    }
}