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
public class DestinosController : ControllerBase
{
    private readonly IDestinoRepository _destinoRepository;
    private readonly IPlantaRepository _plantaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public DestinosController(
        IDestinoRepository destinoRepository,
        IPlantaRepository plantaRepository,
        IUsuarioRepository usuarioRepository)
    {
        _destinoRepository = destinoRepository;
        _plantaRepository = plantaRepository;
        _usuarioRepository = usuarioRepository;
    }

    // -------------------------------------------------------
    // GET /api/Destinos/planta/{idPlanta}
    // -------------------------------------------------------

    [HttpGet("planta/{idPlanta:int}")]
    public async Task<IActionResult> PorPlanta(
        int idPlanta)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
            return BadRequest(
                "La logística no tiene una empresa asociada.");

        var planta =
            await _plantaRepository
                .ObtenerPorIdAsync(
                    idPlanta,
                    usuario.IdTranspor.Value);

        if (planta == null)
            return NotFound(
                "La planta no existe o no pertenece a esta logística.");

        var destinos =
            await _destinoRepository
                .ObtenerPorPlantaAsync(idPlanta);

        return Ok(
            destinos.Select(MapearDestino));
    }

    // -------------------------------------------------------
    // GET /api/Destinos/{id}
    // -------------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(
        int id)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
            return BadRequest(
                "La logística no tiene una empresa asociada.");

        var destino =
            await _destinoRepository
                .ObtenerPorIdAsync(id);

        if (destino == null)
            return NotFound();

        var planta =
            await _plantaRepository
                .ObtenerPorIdAsync(
                    destino.IdPlanta,
                    usuario.IdTranspor.Value);

        if (planta == null)
            return NotFound();

        return Ok(
            MapearDestino(destino));
    }

    // -------------------------------------------------------
    // POST /api/Destinos
    // -------------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Post(
        DestinoRequest request)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
            return BadRequest(
                "La logística no tiene una empresa asociada.");

        var error =
            Validar(request);

        if (error != null)
            return BadRequest(error);

        var planta =
            await _plantaRepository
                .ObtenerPorIdAsync(
                    request.IdPlanta,
                    usuario.IdTranspor.Value);

        if (planta == null)
            return BadRequest(
                "La planta no existe o no pertenece a esta logística.");

        if (!planta.Habilitado)
            return BadRequest(
                "La planta está deshabilitada.");

        Destino destino = new()
        {
            IdPlanta =
                request.IdPlanta,

            DescripDestino =
                request.DescripDestino.Trim(),

            Latitud =
                request.Latitud,

            Longitud =
                request.Longitud
        };

        var id =
            await _destinoRepository
                .CrearAsync(destino);

        return Ok(new
        {
            ok = true,
            idDestino = id,
            mensaje =
                "Destino creado correctamente."
        });
    }

    // -------------------------------------------------------
    // PUT /api/Destinos/{id}
    // -------------------------------------------------------

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(
        int id,
        DestinoRequest request)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
            return BadRequest(
                "La logística no tiene una empresa asociada.");

        var destino =
            await _destinoRepository
                .ObtenerPorIdAsync(id);

        if (destino == null)
            return NotFound();

        // Validamos que el destino actual pertenezca
        // a una planta de esta logística.
        var plantaActual =
            await _plantaRepository
                .ObtenerPorIdAsync(
                    destino.IdPlanta,
                    usuario.IdTranspor.Value);

        if (plantaActual == null)
            return NotFound();

        var error =
            Validar(request);

        if (error != null)
            return BadRequest(error);

        // Validamos también la planta destino
        // por si se intenta mover el destino a otra planta.
        var plantaNueva =
            await _plantaRepository
                .ObtenerPorIdAsync(
                    request.IdPlanta,
                    usuario.IdTranspor.Value);

        if (plantaNueva == null)
            return BadRequest(
                "La planta no existe o no pertenece a esta logística.");

        if (!plantaNueva.Habilitado)
            return BadRequest(
                "La planta está deshabilitada.");

        destino.IdPlanta =
            request.IdPlanta;

        destino.DescripDestino =
            request.DescripDestino.Trim();

        destino.Latitud =
            request.Latitud;

        destino.Longitud =
            request.Longitud;

        await _destinoRepository
            .ActualizarAsync(destino);

        return Ok(new
        {
            ok = true,
            mensaje =
                "Destino actualizado correctamente."
        });
    }

    // -------------------------------------------------------
    // DELETE /api/Destinos/{id}
    // -------------------------------------------------------

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
            return BadRequest(
                "La logística no tiene una empresa asociada.");

        var destino =
            await _destinoRepository
                .ObtenerPorIdAsync(id);

        if (destino == null)
            return NotFound();

        var planta =
            await _plantaRepository
                .ObtenerPorIdAsync(
                    destino.IdPlanta,
                    usuario.IdTranspor.Value);

        if (planta == null)
            return NotFound();

        await _destinoRepository
            .EliminarAsync(id);

        return Ok(new
        {
            ok = true,
            mensaje =
                "Destino eliminado correctamente."
        });
    }

    // -------------------------------------------------------
    // VALIDACIONES
    // -------------------------------------------------------

    private static string? Validar(
        DestinoRequest request)
    {
        if (request.IdPlanta <= 0)
            return "La planta es obligatoria.";

        if (string.IsNullOrWhiteSpace(
            request.DescripDestino))
        {
            return "La descripción del destino es obligatoria.";
        }

        if (request.DescripDestino.Trim().Length > 40)
        {
            return "La descripción del destino no puede superar los 40 caracteres.";
        }

        if (request.Latitud.HasValue !=
            request.Longitud.HasValue)
        {
            return "Debe indicar latitud y longitud.";
        }

        if (request.Latitud.HasValue)
        {
            if (request.Latitud.Value < -90 ||
                request.Latitud.Value > 90)
            {
                return "La latitud no es válida.";
            }

            if (request.Longitud!.Value < -180 ||
                request.Longitud.Value > 180)
            {
                return "La longitud no es válida.";
            }
        }

        return null;
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

    // -------------------------------------------------------
    // MAPEO
    // -------------------------------------------------------

    private static DestinoResponse MapearDestino(
        Destino destino)
    {
        return new DestinoResponse
        {
            IdDestino =
                destino.IdDestino,

            IdPlanta =
                destino.IdPlanta,

            DescripDestino =
                destino.DescripDestino,

            FechaVinculacion =
                destino.FechaVinculacion,

            Latitud =
                destino.Latitud,

            Longitud =
                destino.Longitud
        };
    }
}