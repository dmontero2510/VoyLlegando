using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoyLlegando.Application.DTOs;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Application.Services;

namespace VoyLlegando.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ViajesController : ControllerBase
{
    private readonly IViajeRepository _viajeRepository;
    private readonly ViajeService _viajeService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly RutaService _rutaService;

    public ViajesController(
        IViajeRepository viajeRepository,
        ViajeService viajeService,
        IUsuarioRepository usuarioRepository,
        RutaService rutaService)
    {
        _viajeRepository = viajeRepository;
        _viajeService = viajeService;
        _usuarioRepository = usuarioRepository;
        _rutaService = rutaService;
    }

// -------------------------------------------------------
// GET /api/Viajes/empresa/{idEmpresa}
// LOGISTICA
// -------------------------------------------------------

[HttpGet("empresa/{idEmpresa:int}")]
public async Task<IActionResult> ObtenerEmpresa(
    int idEmpresa)
{
    var usuario =
        await ObtenerUsuarioActual();

    if (usuario == null)
        return Unauthorized();

    if (usuario.Rol != "L")
        return Forbid();

    var empresa =
        await _usuarioRepository
            .ObtenerPorIdAsync(
                idEmpresa);

    if (
        empresa == null ||
        empresa.Rol != "E"
    )
    {
        return NotFound();
    }

    return Ok(new
    {
        idUsuario =
            empresa.IdUsuario,

        nombre =
            empresa.Nombre
    });
}

    // -------------------------------------------------------
    // GET /api/Viajes
    // -------------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        // LOGISTICA
        if (usuario.Rol == "L")
        {
            if (usuario.IdTranspor == null)
                return BadRequest(
                    "La logística no tiene una empresa asociada.");

            var viajes = await _viajeRepository
                .ObtenerPorTransporAsync(
                    usuario.IdTranspor.Value);

            return Ok(
                viajes.Select(MapearViaje));
        }

        // EMPRESA DE TRANSPORTE
        if (usuario.Rol == "E")
        {
            var viajes = await _viajeRepository
                .ObtenerPorCamioneroAsync(
                    usuario.IdUsuario);

            return Ok(
                viajes.Select(MapearViaje));
        }

        return Forbid();
    }

    // -------------------------------------------------------
    // GET /api/Viajes/pendientes
    // LOGISTICA
    // -------------------------------------------------------

    [HttpGet("pendientes")]
    public async Task<IActionResult> Pendientes()
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
            return BadRequest(
                "La logística no tiene una empresa asociada.");

        var viajes = await _viajeRepository
            .ObtenerPorTransporAsync(
                usuario.IdTranspor.Value);

        var pendientes = viajes
            .Where(v => v.Estado == "P")
            .Select(MapearViaje);

        return Ok(pendientes);
    }

    // -------------------------------------------------------
    // GET /api/Viajes/asignados
    // LOGISTICA
    // -------------------------------------------------------

    [HttpGet("asignados")]
    public async Task<IActionResult> Asignados()
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
            return BadRequest(
                "La logística no tiene una empresa asociada.");

        var viajes = await _viajeRepository
            .ObtenerPorTransporAsync(
                usuario.IdTranspor.Value);

        var asignados = viajes
            .Where(v => v.Estado == "A")
            .Select(MapearViaje);

        return Ok(asignados);
    }

    // -------------------------------------------------------
    // GET /api/Viajes/en-curso
    // LOGISTICA
    // -------------------------------------------------------

    [HttpGet("en-curso")]
    public async Task<IActionResult> EnCurso()
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
            return BadRequest(
                "La logística no tiene una empresa asociada.");

        var viajes = await _viajeRepository
            .ObtenerPorTransporAsync(
                usuario.IdTranspor.Value);

        var enCurso = viajes
            .Where(v =>
                v.Estado == "V" ||
                v.Estado == "O" ||
                v.Estado == "R" ||
                v.Estado == "D")
            .Select(MapearViaje);

        return Ok(enCurso);
    }

    // -------------------------------------------------------
    // GET /api/Viajes/terminados
    // LOGISTICA
    // -------------------------------------------------------

    [HttpGet("terminados")]
    public async Task<IActionResult> Terminados()
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
            return BadRequest(
                "La logística no tiene una empresa asociada.");

        var viajes = await _viajeRepository
            .ObtenerPorTransporAsync(
                usuario.IdTranspor.Value);

        var terminados = viajes
            .Where(v => v.Estado == "T")
            .Select(MapearViaje);

        return Ok(terminados);
    }

    // -------------------------------------------------------
    // GET /api/Viajes/empresas-disponibles
    // LOGISTICA
    // -------------------------------------------------------

    [HttpGet("empresas-disponibles")]
    public async Task<IActionResult> EmpresasDisponibles()
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        var usuarios = await _usuarioRepository
            .ObtenerTodosAsync();

        var empresas = usuarios
            .Where(u =>
                u.Rol == "E" &&
                u.Habilitado &&
                u.Estado == "D")
            .Select(u => new
            {
                idUsuario = u.IdUsuario,
                nombre = u.Nombre,
                celular = u.Celular,
                email = u.Email,
                cuit = u.Cuit,
                estado = u.Estado
            });

        return Ok(empresas);
    }

    // -------------------------------------------------------
    // GET /api/Viajes/mis-viajes
    // EMPRESA DE TRANSPORTE
    // -------------------------------------------------------

    [HttpGet("mis-viajes")]
    public async Task<IActionResult> MisViajes()
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "E")
            return Forbid();

        var viajes = await _viajeRepository
            .ObtenerPorCamioneroAsync(
                usuario.IdUsuario);

        return Ok(
            viajes.Select(MapearViaje));
    }

// -------------------------------------------------------
// GET /api/Viajes/pendientes-empresa
// EMPRESA DE TRANSPORTE
// -------------------------------------------------------

[HttpGet("pendientes-empresa")]
public async Task<IActionResult> PendientesEmpresa()
{
    var usuario = await ObtenerUsuarioActual();

    if (usuario == null)
        return Unauthorized();

    if (usuario.Rol != "E")
        return Forbid();

    if (!usuario.Habilitado)
        return BadRequest(
            "La Empresa de Transporte está deshabilitada.");

    if (usuario.Estado != "D")
        return BadRequest(
            "La Empresa de Transporte no está disponible.");

    var viajes =
        await _viajeRepository
            .ObtenerPendientesParaEmpresaAsync(
                usuario.IdUsuario);

    return Ok(viajes);
}

    // -------------------------------------------------------
    // GET /api/Viajes/viaje-actual
    // EMPRESA DE TRANSPORTE
    // -------------------------------------------------------

    [HttpGet("viaje-actual")]
    public async Task<IActionResult> ViajeActual()
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "E")
            return Forbid();

        var viajes = await _viajeRepository
            .ObtenerPorCamioneroAsync(
                usuario.IdUsuario);

        // Viaje operativo actual.
        // A = asignado
        // V = iniciado
        // O = en origen
        // R = en ruta
        // D = en destino

        var viaje = viajes
            .FirstOrDefault(v =>
                v.Estado == "A" ||
                v.Estado == "V" ||
                v.Estado == "O" ||
                v.Estado == "R" ||
                v.Estado == "D");

        if (viaje == null)
        {
            return NotFound(new
            {
                mensaje =
                    "No tiene un viaje activo."
            });
        }

        return Ok(
            MapearViaje(viaje));
    }

    // -------------------------------------------------------
    // GET /api/Viajes/mi-estado
    // EMPRESA DE TRANSPORTE
    // -------------------------------------------------------

    [HttpGet("mi-estado")]
    public async Task<IActionResult> MiEstado()
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "E")
            return Forbid();

        return Ok(new
        {
            idUsuario = usuario.IdUsuario,
            nombre = usuario.Nombre,
            estado = usuario.Estado,
            habilitado = usuario.Habilitado
        });
    }

    // -------------------------------------------------------
    // GET /api/Viajes/{id}
    // -------------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        var viaje = await _viajeRepository
            .ObtenerPorIdAsync(id);

        if (viaje == null)
            return NotFound();

        // LOGISTICA
        if (usuario.Rol == "L")
        {
            if (usuario.IdTranspor !=
                viaje.IdTranspor)
            {
                return Forbid();
            }
        }

        // EMPRESA DE TRANSPORTE
        else if (usuario.Rol == "E")
        {
            if (viaje.IdCamionero !=
                usuario.IdUsuario)
            {
                return Forbid();
            }
        }

        else
        {
            return Forbid();
        }

        return Ok(
            MapearViaje(viaje));
    }

    // -------------------------------------------------------
    // POST /api/Viajes
    // LOGISTICA
    // -------------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Post(
        ViajeRequest request)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
        {
            return BadRequest(
                "La logística no tiene una empresa asociada.");
        }

        try
        {
            var id =
                await _viajeService.CrearAsync(
                    request,
                    usuario.IdUsuario,
                    usuario.IdTranspor.Value);

            return Ok(new
            {
                ok = true,
                id
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                ex.Message);
        }
    }

    // -------------------------------------------------------
    // POST /api/Viajes/{id}/asignar
    // P -> A
    // LOGISTICA
    // -------------------------------------------------------

    [HttpPost("{id:int}/asignar")]
    public async Task<IActionResult> Asignar(
        int id,
        [FromQuery] int idEmpresa)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
        {
            return BadRequest(
                "La logística no tiene una empresa asociada.");
        }

        try
        {
            var resultado =
                await _viajeService.AsignarAsync(
                    id,
                    idEmpresa,
                    usuario.IdTranspor.Value,
                    usuario.IdUsuario);

            if (!resultado)
                return NotFound();

            return Ok(new
            {
                ok = true,
                mensaje =
                    "Viaje asignado correctamente.",
                estado = "A"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                ex.Message);
        }
    }

    // -------------------------------------------------------
    // POST /api/Viajes/{id}/rechazar
    // A -> P
    // EMPRESA
    // -------------------------------------------------------

    [HttpPost("{id:int}/rechazar")]
    public async Task<IActionResult> Rechazar(
        int id)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "E")
            return Forbid();

        try
        {
            var resultado =
                await _viajeService.RechazarAsync(
                    id,
                    usuario.IdUsuario);

            if (!resultado)
                return NotFound();

            return Ok(new
            {
                ok = true,
                mensaje =
                    "Viaje rechazado.",
                estado = "P"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                ex.Message);
        }
    }

    // -------------------------------------------------------
    // POST /api/Viajes/{id}/iniciar
    // A -> V
    // EMPRESA
    // -------------------------------------------------------

    [HttpPost("{id:int}/iniciar")]
    public async Task<IActionResult> Iniciar(
        int id)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "E")
            return Forbid();

        try
        {
            var resultado =
                await _viajeService.IniciarAsync(
                    id,
                    usuario.IdUsuario);

            if (!resultado)
                return NotFound();

            return Ok(new
            {
                ok = true,
                mensaje =
                    "Viaje iniciado.",
                estado = "V"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                ex.Message);
        }
    }

    // -------------------------------------------------------
    // POST /api/Viajes/{id}/llegar-origen
    // V -> O
    // EMPRESA
    // -------------------------------------------------------

    [HttpPost("{id:int}/llegar-origen")]
    public async Task<IActionResult> LlegarOrigen(
        int id)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "E")
            return Forbid();

        try
        {
            var resultado =
                await _viajeService.LlegarOrigenAsync(
                    id,
                    usuario.IdUsuario);

            if (!resultado)
                return NotFound();

            return Ok(new
            {
                ok = true,
                mensaje =
                    "Llegada al origen registrada.",
                estado = "O"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                ex.Message);
        }
    }

    // -------------------------------------------------------
    // POST /api/Viajes/{id}/salir-origen
    // O -> R
    // EMPRESA
    // -------------------------------------------------------

    [HttpPost("{id:int}/salir-origen")]
    public async Task<IActionResult> SalirOrigen(
        int id)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "E")
            return Forbid();

        try
        {
            var resultado =
                await _viajeService.SalirOrigenAsync(
                    id,
                    usuario.IdUsuario);

            if (!resultado)
                return NotFound();

            return Ok(new
            {
                ok = true,
                mensaje =
                    "Salida del origen registrada.",
                estado = "R"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                ex.Message);
        }
    }

    // -------------------------------------------------------
    // POST /api/Viajes/{id}/llegar-destino
    // R -> D
    // EMPRESA
    // -------------------------------------------------------

    [HttpPost("{id:int}/llegar-destino")]
    public async Task<IActionResult> LlegarDestino(
        int id)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "E")
            return Forbid();

        try
        {
            var resultado =
                await _viajeService.LlegarDestinoAsync(
                    id,
                    usuario.IdUsuario);

            if (!resultado)
                return NotFound();

            return Ok(new
            {
                ok = true,
                mensaje =
                    "Llegada al destino registrada.",
                estado = "D"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                ex.Message);
        }
    }

    // -------------------------------------------------------
    // POST /api/Viajes/{id}/terminar
    // D -> T
    // EMPRESA
    // -------------------------------------------------------

    [HttpPost("{id:int}/terminar")]
    public async Task<IActionResult> Terminar(
        int id)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "E")
            return Forbid();

        try
        {
            var resultado =
                await _viajeService.TerminarAsync(
                    id,
                    usuario.IdUsuario);

            if (!resultado)
                return NotFound();

            return Ok(new
            {
                ok = true,
                mensaje =
                    "Viaje terminado.",
                estado = "T"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                ex.Message);
        }
    }

    // -------------------------------------------------------
    // GET /api/Viajes/{id}/recorrido
    // -------------------------------------------------------

    [HttpGet("{id:int}/recorrido")]
    public async Task<IActionResult> Recorrido(
        int id)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        var viaje = await _viajeRepository
            .ObtenerPorIdAsync(id);

        if (viaje == null)
            return NotFound();

        // LOGISTICA
        if (usuario.Rol == "L")
        {
            if (viaje.IdTranspor !=
                usuario.IdTranspor)
            {
                return Forbid();
            }
        }

        // EMPRESA
        else if (usuario.Rol == "E")
        {
            if (viaje.IdCamionero !=
                usuario.IdUsuario)
            {
                return Forbid();
            }
        }

        else
        {
            return Forbid();
        }

        // ---------------------------------------------------
        // VALIDAR COORDENADAS
        // ---------------------------------------------------

        if (!viaje.LatitudOrigen.HasValue ||
            !viaje.LongitudOrigen.HasValue ||
            !viaje.LatitudDestino.HasValue ||
            !viaje.LongitudDestino.HasValue)
        {
            return BadRequest(
                "El viaje no tiene coordenadas de origen y destino.");
        }

        try
        {
            var recorrido =
                await _rutaService.CalcularAsync(
                    viaje.IdViaje,
                    viaje.LatitudOrigen.Value,
                    viaje.LongitudOrigen.Value,
                    viaje.LatitudDestino.Value,
                    viaje.LongitudDestino.Value);

            return Ok(recorrido);
        }
        catch (HttpRequestException)
        {
            return StatusCode(
                503,
                "No se pudo conectar con el servicio de rutas.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(
                ex.Message);
        }
    }

    // -------------------------------------------------------
    // GET /api/Viajes/{id}/ubicacion
    // -------------------------------------------------------

    [HttpGet("{id:int}/ubicacion")]
    public async Task<IActionResult> Ubicacion(
        int id)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        var viaje = await _viajeRepository
            .ObtenerPorIdAsync(id);

        if (viaje == null)
            return NotFound();

        // LOGISTICA
        if (usuario.Rol == "L")
        {
            if (viaje.IdTranspor !=
                usuario.IdTranspor)
            {
                return Forbid();
            }
        }

        // EMPRESA
        else if (usuario.Rol == "E")
        {
            if (viaje.IdCamionero !=
                usuario.IdUsuario)
            {
                return Forbid();
            }
        }

        else
        {
            return Forbid();
        }

        // ---------------------------------------------------
        // EMPRESA ASIGNADA
        // ---------------------------------------------------

        if (!viaje.IdCamionero.HasValue)
        {
            return NotFound(
                "El viaje no tiene una Empresa de Transporte asignada.");
        }

        var empresa =
            await _usuarioRepository.ObtenerPorIdAsync(
                viaje.IdCamionero.Value);

        if (empresa == null)
        {
            return NotFound(
                "La Empresa de Transporte no existe.");
        }

        // ---------------------------------------------------
        // SIN UBICACION
        // ---------------------------------------------------

        if (!empresa.LatitudActual.HasValue ||
            !empresa.LongitudActual.HasValue)
        {
            return NotFound(
                "La Empresa todavía no informó su ubicación.");
        }

        return Ok(new
        {
            idViaje = viaje.IdViaje,
            idEmpresa = empresa.IdUsuario,

            latitud = empresa.LatitudActual,
            longitud = empresa.LongitudActual,

            fechaUbicacion =
                empresa.FechaUbicacion
        });
    }

    // -------------------------------------------------------
    // USUARIO ACTUAL
    // -------------------------------------------------------

    private async Task<
        Domain.Entities.Usuario?>
        ObtenerUsuarioActual()
    {
        var claimId =
            User.FindFirst(
                ClaimTypes.NameIdentifier)
            ?.Value;

        if (!int.TryParse(
            claimId,
            out var idUsuario))
        {
            return null;
        }

        return await _usuarioRepository
            .ObtenerPorIdAsync(
                idUsuario);
    }
// -------------------------------------------------------
// POST /api/Viajes/{id}/tomar
// EMPRESA DE TRANSPORTE
// -------------------------------------------------------

[HttpPost("{id:int}/tomar")]
public async Task<IActionResult> Tomar(
    int id)
{
    var usuario =
        await ObtenerUsuarioActual();

    if (usuario == null)
        return Unauthorized();

    if (usuario.Rol != "E")
        return Forbid();

    try
    {
        await _viajeService
            .TomarAsync(
                id,
                usuario.IdUsuario);

        return Ok(new
        {
            ok = true,
            mensaje =
                "Viaje tomado correctamente."
        });
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new
        {
            ok = false,
            mensaje = ex.Message
        });
    }
}
    // -------------------------------------------------------
    // MAPEO
    // -------------------------------------------------------

    private static ViajeResponse MapearViaje(
        Domain.Entities.Viaje viaje)
    {
        return new ViajeResponse
        {
            IdViaje =
                viaje.IdViaje,

            IdTranspor =
                viaje.IdTranspor,

            IdCamionero =
                viaje.IdCamionero,

            NombreEmpresa =
                viaje.NombreEmpresa,

            Tipo =
                viaje.Tipo,

            FechaPedido =
                viaje.FechaPedido,

            IdCereal =
                viaje.IdCereal,

            IdProduc =
                viaje.IdProduc,

            IdOrigen =
                viaje.IdOrigen,

            IdPlanta =
                viaje.IdPlanta,

            IdDestino =
                viaje.IdDestino,

            Origen =
                viaje.Origen,

            Destino =
                viaje.Destino,

            Ctg =
                viaje.Ctg,

            Kms =
                viaje.Kms,

            Tarifa =
                viaje.Tarifa,

            Estado =
                viaje.Estado,

            FechaAsigna =
                viaje.FechaAsigna,

            FechaTermina =
                viaje.FechaTermina,

            Observaciones =
                viaje.Observaciones,

            Batea =
                viaje.Batea,

            Corta =
                viaje.Corta,

            Larga =
                viaje.Larga,

            LatitudOrigen =
                viaje.LatitudOrigen,

            LongitudOrigen =
                viaje.LongitudOrigen,

            LatitudDestino =
                viaje.LatitudDestino,

            LongitudDestino =
                viaje.LongitudDestino,

            IdUsuario =
                viaje.IdUsuario
        };
    }
}
