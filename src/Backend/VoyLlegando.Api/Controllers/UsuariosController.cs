using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoyLlegando.Application.DTOs;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _repo;

    private readonly IPasswordService _passwordService;

    public UsuariosController(
        IUsuarioRepository repo,
        IPasswordService passwordService)
    {
        _repo = repo;
        _passwordService = passwordService;
    }

    // -------------------------------------------------------
    // GET /api/Usuarios
    // -------------------------------------------------------

    [Authorize(Roles = "S")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var usuarios = await _repo.ObtenerTodosAsync();

        var respuesta = usuarios.Select(MapearUsuario);

        return Ok(respuesta);
    }

    // -------------------------------------------------------
    // GET /api/Usuarios/{id}
    // -------------------------------------------------------

    [Authorize(Roles = "S")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var usuario = await _repo.ObtenerPorIdAsync(id);

        if (usuario == null)
            return NotFound();

        return Ok(MapearUsuario(usuario));
    }

    // -------------------------------------------------------
    // POST /api/Usuarios
    // -------------------------------------------------------

    [Authorize(Roles = "S")]
    [HttpPost]
    public async Task<IActionResult> Post(UsuarioRequest request)
    {
        var errorRol = ValidarRol(request);

        if (errorRol != null)
            return BadRequest(errorRol);

        if (await _repo.ExisteCelularAsync(request.Celular))
            return BadRequest("Ya existe ese celular.");

        var rol = request.Rol.Trim().ToUpper();

        var usuario = new Usuario
        {
            Celular = request.Celular,

            Clave = BCrypt.Net.BCrypt.HashPassword(request.Clave),

            Nombre = request.Nombre,

            Habilitado = true,

            Rol = rol,

            IdTranspor = request.IdTranspor,

            Domicilio = request.Domicilio,

            Iva = request.Iva,

            Cuit = request.Cuit,

            Email = request.Email,

            IdPlanta = request.IdPlanta,

            IdProduc = request.IdProduc,

            PatChasis = request.PatChasis,

            PatAcopla = request.PatAcopla,

            Batea = request.Batea,

            Corta = request.Corta,

            Larga = request.Larga,

            Escala = request.Escala,

            // El estado solamente corresponde a E
            // y toda empresa nueva comienza Disponible.
            Estado = rol == "E" ? "D" : null
        };

        var id = await _repo.CrearAsync(usuario);

        return Ok(new
        {
            ok = true,
            id
        });
    }

    // -------------------------------------------------------
    // POST /api/Usuarios/{id}/restablecer-clave
    // -------------------------------------------------------

    [Authorize(Roles = "S")]
    [HttpPost("{id:int}/restablecer-clave")]
    public async Task<IActionResult> RestablecerClave(
        int id,
        RestablecerClaveRequest request)
    {
        var usuario =
            await _repo.ObtenerPorIdAsync(id);


        if (usuario == null)
            return NotFound();


        if (string.IsNullOrWhiteSpace(request.ClaveTemporal))
            return BadRequest("Ingrese la clave temporal.");


        if (
            request.ClaveTemporal.Length < 6 ||
            request.ClaveTemporal.Length > 72
        )
        {
            return BadRequest(
                "La clave temporal debe tener entre 6 y 72 caracteres.");
        }


        if (request.ClaveTemporal != request.RepetirClave)
            return BadRequest("Las claves temporales no coinciden.");


        await _repo.ActualizarClaveAsync(
            id,
            _passwordService.GenerarHash(
                request.ClaveTemporal),
            true);


        return Ok(new
        {
            ok = true,
            mensaje = "Clave restablecida. El usuario deberá cambiarla en su próximo ingreso."
        });
    }


    // -------------------------------------------------------
    // PUT /api/Usuarios/{id}
    // -------------------------------------------------------

    [Authorize(Roles = "S")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(
    int id,
    UsuarioRequest request)
    {
        var usuario = await _repo.ObtenerPorIdAsync(id);

        if (usuario == null)
            return NotFound();

        var errorRol = ValidarRol(request);

        if (errorRol != null)
            return BadRequest(errorRol);

        if (await _repo.ExisteCelularAsync(
                request.Celular,
                id))
        {
            return BadRequest("Ya existe ese celular.");
        }

        var nuevoRol = request.Rol.Trim().ToUpper();

        // ---------------------------------------------------
        // No permitir modificar el estado desde PUT
        // ---------------------------------------------------

        if (usuario.Rol == "E" &&
            nuevoRol == "E" &&
            !string.IsNullOrWhiteSpace(request.Estado))
        {
            return BadRequest(
                "El estado de la Empresa de Transporte no puede modificarse manualmente.");
        }

        // Si cambia de otro rol a E, comienza disponible.
        if (usuario.Rol != "E" && nuevoRol == "E")
        {
            usuario.Estado = "D";
        }

        // Si deja de ser E, pierde el estado de empresa.
        if (usuario.Rol == "E" && nuevoRol != "E")
        {
            if (usuario.Estado == "V")
            {
                return BadRequest(
                    "No se puede cambiar el rol de una Empresa de Transporte mientras está viajando.");
            }

            usuario.Estado = null;
        }

        usuario.Celular = request.Celular;
        usuario.Nombre = request.Nombre;
        usuario.Rol = nuevoRol;

        usuario.IdTranspor = request.IdTranspor;

        usuario.Domicilio = request.Domicilio;
        usuario.Iva = request.Iva;
        usuario.Cuit = request.Cuit;
        usuario.Email = request.Email;

        usuario.IdPlanta = request.IdPlanta;
        usuario.IdProduc = request.IdProduc;

        usuario.PatChasis = request.PatChasis;
        usuario.PatAcopla = request.PatAcopla;

        usuario.Batea = request.Batea;
        usuario.Corta = request.Corta;
        usuario.Larga = request.Larga;
        usuario.Escala = request.Escala;

        await _repo.ActualizarAsync(usuario);

        return Ok(new
        {
            ok = true,
            id
        });
    }

    // -------------------------------------------------------
    // DELETE /api/Usuarios/{id}
    // -------------------------------------------------------

    [Authorize(Roles = "S")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuario = await _repo.ObtenerPorIdAsync(id);

        if (usuario == null)
            return NotFound();

        await _repo.BajaAsync(id);

        return Ok(new
        {
            ok = true
        });
    }

// -------------------------------------------------------
// POST /api/Usuarios/{id}/habilitar
// -------------------------------------------------------

[Authorize(Roles = "S")]
[HttpPost("{id:int}/habilitar")]
public async Task<IActionResult> Habilitar(
    int id)
{
    var usuario =
        await _repo.ObtenerPorIdAsync(id);

    if (usuario == null)
        return NotFound();


    if (usuario.Habilitado)
    {
        return Ok(new
        {
            ok = true,
            mensaje =
                "El usuario ya está habilitado."
        });
    }


    usuario.Habilitado =
        true;


    // Si es Empresa de Transporte y no tiene
    // estado operativo, vuelve disponible.
    if (
        usuario.Rol == "E" &&
        string.IsNullOrWhiteSpace(
            usuario.Estado
        )
    )
    {
        usuario.Estado =
            "D";
    }


    await _repo.ActualizarAsync(
        usuario
    );


    return Ok(new
    {
        ok = true,
        mensaje =
            "Usuario habilitado correctamente."
    });
}

    // -------------------------------------------------------
    // GET /api/Usuarios/mi-perfil
    // EMPRESA DE TRANSPORTE
    // -------------------------------------------------------

    [Authorize(Roles = "E")]
    [HttpGet("mi-perfil")]
    public async Task<IActionResult> MiPerfil()
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        return Ok(new
        {
            nombre = usuario.Nombre,
            domicilio = usuario.Domicilio,
            cuit = usuario.Cuit,
            celular = usuario.Celular,
            email = usuario.Email,
            patChasis = usuario.PatChasis,
            patAcopla = usuario.PatAcopla,
            batea = usuario.Batea,
            corta = usuario.Corta,
            larga = usuario.Larga,
            escala = usuario.Escala,
            estado = usuario.Estado
        });
    }

    // -------------------------------------------------------
    // PUT /api/Usuarios/mi-perfil
    // EMPRESA DE TRANSPORTE
    // -------------------------------------------------------

    [Authorize(Roles = "E")]
    [HttpPut("mi-perfil")]
    public async Task<IActionResult> ActualizarMiPerfil(
        MiPerfilRequest request)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (!usuario.Habilitado)
            return BadRequest("La Empresa de Transporte está deshabilitada.");

        var nombre = request.Nombre?.Trim() ?? "";
        var celular = request.Celular?.Trim() ?? "";
        var cuit = request.Cuit?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(nombre))
            return BadRequest("Ingrese el nombre de la empresa.");

        if (string.IsNullOrWhiteSpace(celular))
            return BadRequest("Ingrese el celular.");

        if (await _repo.ExisteCelularAsync(celular, usuario.IdUsuario))
            return BadRequest("Ya existe ese celular.");

        usuario.Nombre = nombre;
        usuario.Domicilio = request.Domicilio?.Trim();
        usuario.Cuit = cuit;
        usuario.Celular = celular;
        usuario.Email = request.Email?.Trim() ?? "";
        usuario.PatChasis = request.PatChasis?.Trim();
        usuario.PatAcopla = request.PatAcopla?.Trim();
        usuario.Batea = request.Batea;
        usuario.Corta = request.Corta;
        usuario.Larga = request.Larga;
        usuario.Escala = request.Escala;

        await _repo.ActualizarAsync(usuario);

        return Ok(new
        {
            ok = true,
            mensaje = "Perfil actualizado correctamente."
        });
    }

    // -------------------------------------------------------
    // POST /api/Usuarios/mi-disponibilidad
    // -------------------------------------------------------

    [HttpPost("mi-disponibilidad")]
    public async Task<IActionResult> MiDisponibilidad()
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        // Solamente Empresa de Transporte
        if (usuario.Rol != "E")
            return Forbid();

        if (!usuario.Habilitado)
            return BadRequest(
                "La Empresa de Transporte está deshabilitada.");

        // Si está viajando, no puede ponerse disponible
        if (usuario.Estado == "V")
            return BadRequest(
                "La Empresa de Transporte tiene un viaje en curso.");

        // Ya está disponible
        if (usuario.Estado == "D")
        {
            return Ok(new
            {
                ok = true,
                mensaje = "La Empresa de Transporte ya está disponible.",
                estado = usuario.Estado
            });
        }

        // Solamente N -> D
        if (usuario.Estado != "N")
            return BadRequest(
                "La Empresa de Transporte no puede cambiar a disponible desde su estado actual.");

        usuario.Estado = "D";

        await _repo.ActualizarAsync(usuario);

        return Ok(new
        {
            ok = true,
            mensaje = "La Empresa de Transporte ahora está disponible.",
            estado = usuario.Estado
        });
    }
// -------------------------------------------------------
// POST /api/Usuarios/mi-ubicacion
// -------------------------------------------------------

[HttpPost("mi-ubicacion")]
public async Task<IActionResult> MiUbicacion(
    UbicacionRequest request)
{
    var usuario =
        await ObtenerUsuarioActual();

    if (usuario == null)
        return Unauthorized();

    // Solamente Empresa de Transporte
    if (usuario.Rol != "E")
        return Forbid();

    if (!usuario.Habilitado)
    {
        return BadRequest(
            "La Empresa de Transporte está deshabilitada.");
    }

    // ---------------------------------------------------
    // VALIDAR COORDENADAS
    // ---------------------------------------------------

    if (request.Latitud < -90 ||
        request.Latitud > 90)
    {
        return BadRequest(
            "Latitud inválida.");
    }

    if (request.Longitud < -180 ||
        request.Longitud > 180)
    {
        return BadRequest(
            "Longitud inválida.");
    }

    // ---------------------------------------------------
    // ACTUALIZAR POSICIÓN
    // ---------------------------------------------------

    usuario.LatitudActual =
        request.Latitud;

    usuario.LongitudActual =
        request.Longitud;

    usuario.FechaUbicacion =
        DateTime.UtcNow;

    await _repo.ActualizarAsync(
        usuario);

    return Ok(new
    {
        ok = true,

        latitud =
            usuario.LatitudActual,

        longitud =
            usuario.LongitudActual,

        fechaUbicacion =
            usuario.FechaUbicacion
    });
}

    // -------------------------------------------------------
    // OBTENER USUARIO ACTUAL
    // -------------------------------------------------------

    private async Task<Usuario?> ObtenerUsuarioActual()
    {
        var claimId = User.FindFirst(
            ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(claimId, out var idUsuario))
            return null;

        return await _repo.ObtenerPorIdAsync(idUsuario);
    }

    // -------------------------------------------------------
    // VALIDACIÓN DE ROLES
    // -------------------------------------------------------

    private static string? ValidarRol(UsuarioRequest request)
    {
        var rol = request.Rol?.Trim().ToUpper();

if (rol != "L" &&
    rol != "E" &&
    rol != "A" &&
    rol != "P" &&
    rol != "S")
        {
            return "Rol inválido. Los roles permitidos son L, E, A, S y P.";
        }

        // ---------------------------------------------------
        // L = LOGISTICA
        // ---------------------------------------------------

        if (rol == "L")
        {
            if (!request.IdTranspor.HasValue)
                return "Para el rol L debe indicar idTranspor.";

            if (request.IdPlanta.HasValue)
                return "Un usuario L no puede tener idPlanta.";

            if (request.IdProduc.HasValue)
                return "Un usuario L no puede tener idProduc.";

            if (TieneDatosVehiculo(request))
                return "Un usuario L no puede tener datos de vehículo.";
        }

        // ---------------------------------------------------
        // E = EMPRESA DE TRANSPORTE
        // ---------------------------------------------------

        if (rol == "E")
        {
            if (request.IdTranspor.HasValue)
                return "Un usuario E no puede tener idTranspor.";

            if (request.IdPlanta.HasValue)
                return "Un usuario E no puede tener idPlanta.";

            if (request.IdProduc.HasValue)
                return "Un usuario E no puede tener idProduc.";
        }

        // ---------------------------------------------------
        // A = ACOPIOS / PLANTAS
        // ---------------------------------------------------

        if (rol == "A")
        {
            if (!request.IdPlanta.HasValue)
                return "Para el rol A debe indicar idPlanta.";

            if (request.IdTranspor.HasValue)
                return "Un usuario A no puede tener idTranspor.";

            if (request.IdProduc.HasValue)
                return "Un usuario A no puede tener idProduc.";

            if (TieneDatosVehiculo(request))
                return "Un usuario A no puede tener datos de vehículo.";
        }

        // ---------------------------------------------------
        // P = PRODUCTOR AGROPECUARIO
        // ---------------------------------------------------

        if (rol == "P")
        {
            if (!request.IdProduc.HasValue)
                return "Para el rol P debe indicar idProduc.";

            if (request.IdTranspor.HasValue)
                return "Un usuario P no puede tener idTranspor.";

            if (request.IdPlanta.HasValue)
                return "Un usuario P no puede tener idPlanta.";

            if (TieneDatosVehiculo(request))
                return "Un usuario P no puede tener datos de vehículo.";
        }

// ---------------------------------------------------
// S = SYSTEM ADMINISTRATOR
// ---------------------------------------------------

if (rol == "S")
{
    if (request.IdTranspor.HasValue)
        return "Un usuario S no puede tener idTranspor.";

    if (request.IdPlanta.HasValue)
        return "Un usuario S no puede tener idPlanta.";

    if (request.IdProduc.HasValue)
        return "Un usuario S no puede tener idProduc.";

    if (TieneDatosVehiculo(request))
        return "Un usuario S no puede tener datos de vehículo.";
}
        return null;
    }

    // -------------------------------------------------------
    // DATOS DE VEHÍCULO
    // -------------------------------------------------------

    private static bool TieneDatosVehiculo(UsuarioRequest request)
    {
        return
            !string.IsNullOrWhiteSpace(request.PatChasis) ||
            !string.IsNullOrWhiteSpace(request.PatAcopla) ||
            request.Batea ||
            request.Corta ||
            request.Larga ||
            request.Escala;
    }

    // -------------------------------------------------------
    // MAPEO
    // -------------------------------------------------------

    private static UsuarioResponse MapearUsuario(Usuario usuario)
    {
        return new UsuarioResponse
        {
            IdUsuario = usuario.IdUsuario,
            Nombre = usuario.Nombre,
            Domicilio = usuario.Domicilio,
            Iva = usuario.Iva,
            Cuit = usuario.Cuit,
            Celular = usuario.Celular,
            Email = usuario.Email,
            Rol = usuario.Rol,
            Habilitado = usuario.Habilitado,
            DebeCambiarClave = usuario.DebeCambiarClave,

            IdTranspor = usuario.IdTranspor,
            IdPlanta = usuario.IdPlanta,
            IdProduc = usuario.IdProduc,

            PatChasis = usuario.PatChasis,
            PatAcopla = usuario.PatAcopla,

            Batea = usuario.Batea,
            Corta = usuario.Corta,
            Larga = usuario.Larga,
            Escala = usuario.Escala,

            Estado = usuario.Estado
        };
    }
}

public class MiPerfilRequest
{
    public string Nombre { get; set; } = "";
    public string? Domicilio { get; set; }
    public string Cuit { get; set; } = "";
    public string Celular { get; set; } = "";
    public string? Email { get; set; }
    public string? PatChasis { get; set; }
    public string? PatAcopla { get; set; }
    public bool Batea { get; set; }
    public bool Corta { get; set; }
    public bool Larga { get; set; }
    public bool Escala { get; set; }
}
