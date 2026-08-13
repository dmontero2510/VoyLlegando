using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoyLlegando.Application.Interfaces;

namespace VoyLlegando.Api.Controllers;

[ApiController]
[Authorize(Roles = "E")]
[Route("api/[controller]")]
public class LogisticasCamionController : ControllerBase
{
    private readonly ILogisticaCamionRepository
        _logisticaCamionRepository;

    private readonly IUsuarioRepository
        _usuarioRepository;

    private readonly ILogisticaRepository
        _logisticaRepository;


    public LogisticasCamionController(
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
    // LOGISTICAS VINCULADAS A LA EMPRESA
    // =======================================================

    [HttpGet("vinculadas")]
    public async Task<IActionResult>
        ObtenerVinculadas()
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


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
    // LOGISTICAS DISPONIBLES PARA VINCULAR
    // =======================================================

    [HttpGet("disponibles")]
    public async Task<IActionResult>
        ObtenerDisponibles()
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


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
    // VINCULAR EMPRESA CON LOGISTICA
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
                    "La Empresa fue vinculada correctamente con la Logística."
            });
    }


    // =======================================================
    // DESVINCULAR EMPRESA DE LOGISTICA
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
                "La Empresa no está vinculada con esta Logística.");
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