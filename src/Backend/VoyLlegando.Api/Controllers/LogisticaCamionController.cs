using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoyLlegando.Application.Interfaces;

namespace VoyLlegando.Api.Controllers;

[ApiController]
[Authorize(Roles = "E,L")]
[Route("api/[controller]")]
public class LogisticaCamionController : ControllerBase
{
    private readonly ILogisticaCamionRepository
        _logisticaCamionRepository;

    private readonly IUsuarioRepository
        _usuarioRepository;

    private readonly ILogisticaRepository
        _logisticaRepository;


    public LogisticaCamionController(
        ILogisticaCamionRepository logisticaCamionRepository,
        IUsuarioRepository usuarioRepository,
        ILogisticaRepository logisticaRepository)
    {
        _logisticaCamionRepository =
            logisticaCamionRepository;

        _usuarioRepository =
            usuarioRepository;

        _logisticaRepository =
            logisticaRepository;
    }


    // =======================================================
    // E - LOGISTICAS ACEPTADAS / VINCULADAS
    // =======================================================

    [HttpGet("vinculadas")]
    public async Task<IActionResult>
        ObtenerVinculadas()
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


        if (usuario.Rol != "E")
            return Forbid();


        if (!usuario.Habilitado)
        {
            return BadRequest(
                "La Empresa de Transporte está deshabilitada.");
        }


        var logisticas =
            await _logisticaCamionRepository
                .ObtenerVinculadasAsync(
                    usuario.IdUsuario);


        return Ok(logisticas);
    }


    // =======================================================
    // E - LOGISTICAS DISPONIBLES PARA SOLICITAR VINCULACION
    // =======================================================

    [HttpGet("disponibles")]
    public async Task<IActionResult>
        ObtenerDisponibles()
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


        if (usuario.Rol != "E")
            return Forbid();


        if (!usuario.Habilitado)
        {
            return BadRequest(
                "La Empresa de Transporte está deshabilitada.");
        }


        var logisticas =
            await _logisticaCamionRepository
                .ObtenerDisponiblesAsync(
                    usuario.IdUsuario);


        return Ok(logisticas);
    }


    // =======================================================
    // E - SOLICITAR VINCULACION CON LOGISTICA
    // =======================================================

    [HttpPost("{idTranspor:int}/vincular")]
    public async Task<IActionResult>
        Vincular(
            int idTranspor)
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


        if (usuario.Rol != "E")
            return Forbid();


        if (!usuario.Habilitado)
        {
            return BadRequest(
                "La Empresa de Transporte está deshabilitada.");
        }


        var logistica =
            await _logisticaRepository
                .ObtenerPorIdAsync(
                    idTranspor);


        if (logistica == null)
        {
            return NotFound(
                "La Logística no existe.");
        }


        if (!logistica.Habilitado)
        {
            return BadRequest(
                "La Logística está deshabilitada.");
        }


        var yaVinculado =
            await _logisticaCamionRepository
                .EstaVinculadoAsync(
                    idTranspor,
                    usuario.IdUsuario);


        if (yaVinculado)
        {
            return BadRequest(
                "La Empresa ya está vinculada con esta Logística.");
        }


        await _logisticaCamionRepository
            .VincularAsync(
                idTranspor,
                usuario.IdUsuario);


        return Ok(
            new
            {
                ok = true,

                mensaje =
                    "La solicitud fue enviada a la Logística y está pendiente de aprobación."
            });
    }


    // =======================================================
    // E - DESVINCULAR EMPRESA DE LOGISTICA
    // =======================================================

    [HttpDelete("{idTranspor:int}/desvincular")]
    public async Task<IActionResult>
        Desvincular(
            int idTranspor)
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


        if (usuario.Rol != "E")
            return Forbid();


        if (!usuario.Habilitado)
        {
            return BadRequest(
                "La Empresa de Transporte está deshabilitada.");
        }


        var vinculado =
            await _logisticaCamionRepository
                .EstaVinculadoAsync(
                    idTranspor,
                    usuario.IdUsuario);


        if (!vinculado)
        {
            return BadRequest(
                "La Empresa no tiene una vinculación activa con esta Logística.");
        }


        await _logisticaCamionRepository
            .DesvincularAsync(
                idTranspor,
                usuario.IdUsuario);


        return Ok(
            new
            {
                ok = true,

                mensaje =
                    "La Empresa fue desvinculada de la Logística."
            });
    }


    // =======================================================
    // L - SOLICITUDES PENDIENTES
    // =======================================================

    [HttpGet("solicitudes-pendientes")]
    public async Task<IActionResult>
        ObtenerSolicitudesPendientes()
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


        if (usuario.Rol != "L")
            return Forbid();


        if (!usuario.Habilitado)
        {
            return BadRequest(
                "La Logística está deshabilitada.");
        }


        if (usuario.IdTranspor == null)
        {
            return BadRequest(
                "La Logística no tiene una empresa asociada.");
        }


        var solicitudes =
            await _logisticaCamionRepository
                .ObtenerSolicitudesPendientesAsync(
                    usuario.IdTranspor.Value);


        return Ok(solicitudes);
    }


    // =======================================================
    // L - ACEPTAR SOLICITUD
    // P -> A
    // =======================================================

    [HttpPost("solicitudes/{idUsuario:int}/aceptar")]
    public async Task<IActionResult>
        AceptarSolicitud(
            int idUsuario)
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


        if (usuario.Rol != "L")
            return Forbid();


        if (!usuario.Habilitado)
        {
            return BadRequest(
                "La Logística está deshabilitada.");
        }


        if (usuario.IdTranspor == null)
        {
            return BadRequest(
                "La Logística no tiene una empresa asociada.");
        }


        var empresa =
            await _usuarioRepository
                .ObtenerPorIdAsync(
                    idUsuario);


        if (
            empresa == null ||
            empresa.Rol != "E"
        )
        {
            return NotFound(
                "La Empresa de Transporte no existe.");
        }


        await _logisticaCamionRepository
            .AceptarSolicitudAsync(
                usuario.IdTranspor.Value,
                idUsuario);


        return Ok(
            new
            {
                ok = true,

                mensaje =
                    "La solicitud fue aceptada correctamente."
            });
    }


    // =======================================================
    // L - RECHAZAR SOLICITUD
    // P -> R
    // =======================================================

    [HttpPost("solicitudes/{idUsuario:int}/rechazar")]
    public async Task<IActionResult>
        RechazarSolicitud(
            int idUsuario)
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


        if (usuario.Rol != "L")
            return Forbid();


        if (!usuario.Habilitado)
        {
            return BadRequest(
                "La Logística está deshabilitada.");
        }


        if (usuario.IdTranspor == null)
        {
            return BadRequest(
                "La Logística no tiene una empresa asociada.");
        }


        var empresa =
            await _usuarioRepository
                .ObtenerPorIdAsync(
                    idUsuario);


        if (
            empresa == null ||
            empresa.Rol != "E"
        )
        {
            return NotFound(
                "La Empresa de Transporte no existe.");
        }


        await _logisticaCamionRepository
            .RechazarSolicitudAsync(
                usuario.IdTranspor.Value,
                idUsuario);


        return Ok(
            new
            {
                ok = true,

                mensaje =
                    "La solicitud fue rechazada."
            });
    }


    // =======================================================
    // L - TODAS LAS RELACIONES
    // A = ACEPTADA
    // R = RECHAZADA
    // B = BAJA / BLOQUEADA
    // =======================================================

    [HttpGet("relaciones")]
    public async Task<IActionResult>
        ObtenerRelaciones()
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


        if (usuario.Rol != "L")
            return Forbid();


        if (!usuario.Habilitado)
        {
            return BadRequest(
                "La Logística está deshabilitada.");
        }


        if (usuario.IdTranspor == null)
        {
            return BadRequest(
                "La Logística no tiene una empresa asociada.");
        }


        var relaciones =
            await _logisticaCamionRepository
                .ObtenerRelacionesAsync(
                    usuario.IdTranspor.Value);


        return Ok(relaciones);
    }


    // =======================================================
    // L - BLOQUEAR RELACION
    // A -> B
    // =======================================================

    [HttpPost("relaciones/{idUsuario:int}/bloquear")]
    public async Task<IActionResult>
        BloquearRelacion(
            int idUsuario)
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


        if (usuario.Rol != "L")
            return Forbid();


        if (!usuario.Habilitado)
        {
            return BadRequest(
                "La Logística está deshabilitada.");
        }


        if (usuario.IdTranspor == null)
        {
            return BadRequest(
                "La Logística no tiene una empresa asociada.");
        }


        var empresa =
            await _usuarioRepository
                .ObtenerPorIdAsync(
                    idUsuario);


        if (
            empresa == null ||
            empresa.Rol != "E"
        )
        {
            return NotFound(
                "La Empresa de Transporte no existe.");
        }


        var actualizado =
            await _logisticaCamionRepository
                .BloquearRelacionAsync(
                    usuario.IdTranspor.Value,
                    idUsuario);


        if (!actualizado)
        {
            return BadRequest(
                "La relación no está aceptada o ya se encuentra bloqueada.");
        }


        return Ok(
            new
            {
                ok = true,

                mensaje =
                    "La Empresa de Transporte fue bloqueada para esta Logística."
            });
    }


    // =======================================================
    // L - REHABILITAR RELACION
    // R/B -> A
    // =======================================================

    [HttpPost("relaciones/{idUsuario:int}/rehabilitar")]
    public async Task<IActionResult>
        RehabilitarRelacion(
            int idUsuario)
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


        if (usuario.Rol != "L")
            return Forbid();


        if (!usuario.Habilitado)
        {
            return BadRequest(
                "La Logística está deshabilitada.");
        }


        if (usuario.IdTranspor == null)
        {
            return BadRequest(
                "La Logística no tiene una empresa asociada.");
        }


        var empresa =
            await _usuarioRepository
                .ObtenerPorIdAsync(
                    idUsuario);


        if (
            empresa == null ||
            empresa.Rol != "E"
        )
        {
            return NotFound(
                "La Empresa de Transporte no existe.");
        }


        var actualizado =
            await _logisticaCamionRepository
                .RehabilitarRelacionAsync(
                    usuario.IdTranspor.Value,
                    idUsuario);


        if (!actualizado)
        {
            return BadRequest(
                "La relación no está rechazada ni bloqueada.");
        }


        return Ok(
            new
            {
                ok = true,

                mensaje =
                    "La Empresa de Transporte fue rehabilitada correctamente."
            });
    }


    // =======================================================
    // USUARIO ACTUAL
    // =======================================================

    private async Task<
        VoyLlegando.Domain.Entities.Usuario?>
        ObtenerUsuarioActual()
    {
        var claim =
            User.FindFirst(
                ClaimTypes.NameIdentifier);


        if (
            claim == null ||
            !int.TryParse(
                claim.Value,
                out var idUsuario)
        )
        {
            return null;
        }


        return await _usuarioRepository
            .ObtenerPorIdAsync(
                idUsuario);
    }
}
