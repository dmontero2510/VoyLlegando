using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;
using VoyLlegando.Application.DTOs;

namespace VoyLlegando.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CerealesController
    : ControllerBase
{
    private readonly ICerealRepository _cerealRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public CerealesController(
        ICerealRepository cerealRepository,
        IUsuarioRepository usuarioRepository)
    {
        _cerealRepository = cerealRepository;
        _usuarioRepository = usuarioRepository;
    }

    // -------------------------------------------------------
    // GET /api/Cereales
    // TODOS - ABM
    // -------------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var usuario =
            await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        var cereales =
            await _cerealRepository
                .ObtenerTodosAsync();

        return Ok(
            cereales.Select(
                c => new
                {
                    idCereal =
                        c.IdCereal,

                    nombre =
                        c.NombreCereal,

                    habilitado =
                        c.Habilitado
                }
            )
        );
    }

    // -------------------------------------------------------
    // GET /api/Cereales/habilitados
    // PARA COMBO DE VIAJES
    // -------------------------------------------------------

    [HttpGet("habilitados")]
    public async Task<IActionResult>
        Habilitados()
    {
        var usuario =
            await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        var cereales =
            await _cerealRepository
                .ObtenerHabilitadosAsync();

        return Ok(
            cereales.Select(
                c => new
                {
                    idCereal =
                        c.IdCereal,

                    nombre =
                        c.NombreCereal
                }
            )
        );
    }

    // -------------------------------------------------------
    // GET /api/Cereales/{id}
    // -------------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(
        int id)
    {
        var usuario =
            await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        var cereal =
            await _cerealRepository
                .ObtenerPorIdAsync(id);

        if (cereal == null)
            return NotFound();

        return Ok(new
        {
            idCereal =
                cereal.IdCereal,

            nombre =
                cereal.NombreCereal,

            habilitado =
                cereal.Habilitado
        });
    }

    // -------------------------------------------------------
    // POST /api/Cereales
    // -------------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Post(
        CerealRequest request)
    {
        var usuario =
            await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (request.IdCereal <= 0)
        {
            return BadRequest(
                "El código del cereal es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(
            request.Nombre))
        {
            return BadRequest(
                "El nombre del cereal es obligatorio.");
        }

        var existente =
            await _cerealRepository
                .ObtenerPorIdAsync(
                    request.IdCereal);

        if (existente != null)
        {
            return BadRequest(
                "Ya existe un cereal con ese código.");
        }

        Cereal cereal = new()
        {
            IdCereal =
                request.IdCereal,

            NombreCereal =
                request.Nombre.Trim(),

            Habilitado =
                request.Habilitado
        };

        await _cerealRepository
            .CrearAsync(cereal);

        return Ok(new
        {
            ok = true,
            mensaje =
                "Cereal creado correctamente."
        });
    }

    // -------------------------------------------------------
    // PUT /api/Cereales/{id}
    // -------------------------------------------------------

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(
        int id,
        CerealRequest request)
    {
        var usuario =
            await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        var cereal =
            await _cerealRepository
                .ObtenerPorIdAsync(id);

        if (cereal == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(
            request.Nombre))
        {
            return BadRequest(
                "El nombre del cereal es obligatorio.");
        }

        cereal.NombreCereal =
            request.Nombre.Trim();

        cereal.Habilitado =
            request.Habilitado;

        await _cerealRepository
            .ActualizarAsync(cereal);

        return Ok(new
        {
            ok = true,
            mensaje =
                "Cereal actualizado correctamente."
        });
    }

    // -------------------------------------------------------
    // DELETE /api/Cereales/{id}
    // BAJA LOGICA
    // -------------------------------------------------------

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var usuario =
            await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        var cereal =
            await _cerealRepository
                .ObtenerPorIdAsync(id);

        if (cereal == null)
            return NotFound();

        await _cerealRepository
            .BajaAsync(id);

        return Ok(new
        {
            ok = true,
            mensaje =
                "Cereal deshabilitado correctamente."
        });
    }

    // -------------------------------------------------------
    // USUARIO ACTUAL
    // -------------------------------------------------------

    private async Task<Usuario?>
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
}