using VoyLlegando.Application.DTOs;
using VoyLlegando.Application.Interfaces;

namespace VoyLlegando.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IPasswordService passwordService,
        IJwtService jwtService)
    {
        _usuarioRepository = usuarioRepository;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    // -------------------------------------------------------
    // LOGIN
    // -------------------------------------------------------

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var usuario = await _usuarioRepository
            .ObtenerPorCelularAsync(request.Celular);

        // ---------------------------------------------------
        // USUARIO INEXISTENTE
        // ---------------------------------------------------

        if (usuario == null)
        {
            return new LoginResponse
            {
                Success = false,
                Mensaje = "Usuario inexistente"
            };
        }

        // ---------------------------------------------------
        // USUARIO DESHABILITADO
        // ---------------------------------------------------

        if (!usuario.Habilitado)
        {
            return new LoginResponse
            {
                Success = false,
                Mensaje = "Usuario deshabilitado"
            };
        }

        // ---------------------------------------------------
        // ROLES PERMITIDOS PARA LOGIN
        //
        // L = LOGISTICA
        // E = EMPRESA DE TRANSPORTE
        //
        // A y P todavía no pueden ingresar.
        // ---------------------------------------------------

        if (usuario.Rol != "L" &&
            usuario.Rol != "E" &&
            usuario.Rol != "S")
        {
            return new LoginResponse
            {
                Success = false,
                Mensaje = "Este tipo de usuario todavía no puede ingresar."
            };
        }

        // ---------------------------------------------------
        // VALIDAR CLAVE
        // ---------------------------------------------------

        if (!_passwordService.Verificar(
                request.Clave,
                usuario.Clave))
        {
            return new LoginResponse
            {
                Success = false,
                Mensaje = "Clave incorrecta"
            };
        }

        // ---------------------------------------------------
        // GENERAR JWT
        // ---------------------------------------------------

        var token = _jwtService.GenerarToken(usuario);

        // ---------------------------------------------------
        // RESPUESTA
        // ---------------------------------------------------

        return new LoginResponse
        {
            Success = true,
            Token = token,
            Mensaje = "Login correcto",

            Usuario = new PerfilResponse
            {
                Id = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Telefono = usuario.Celular,
                Rol = usuario.Rol,

                PatChasis = usuario.PatChasis,
                PatAcoplado = usuario.PatAcopla,

                Batea = usuario.Batea ?? false,
                Corta = usuario.Corta ?? false,
                Larga = usuario.Larga ?? false,
                Escala = usuario.Escala ?? false,

                Estado = usuario.Estado ?? ""
            }
        };
    }
}