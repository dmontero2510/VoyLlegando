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
public class PlantasController : ControllerBase
{
    private readonly IPlantaRepository _plantaRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public PlantasController(
        IPlantaRepository plantaRepository,
        IUsuarioRepository usuarioRepository)
    {
        _plantaRepository = plantaRepository;
        _usuarioRepository = usuarioRepository;
    }

    // -------------------------------------------------------
    // GET /api/Plantas
    // -------------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
            return BadRequest(
                "La logística no tiene una empresa asociada.");

        var plantas =
            await _plantaRepository.ObtenerTodosAsync(
                usuario.IdTranspor.Value);

        return Ok(
            plantas.Select(MapearPlanta));
    }

    // -------------------------------------------------------
    // GET /api/Plantas/{id}
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

        var planta =
            await _plantaRepository.ObtenerPorIdAsync(
                id,
                usuario.IdTranspor.Value);

        if (planta == null)
            return NotFound();

        return Ok(
            MapearPlanta(planta));
    }

    // -------------------------------------------------------
    // POST /api/Plantas
    // -------------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Post(
        PlantaRequest request)
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

        var cuit =
            LimpiarCuit(request.Cuit);

        Planta planta = new()
        {
            IdTranspor =
                usuario.IdTranspor.Value,

            Nombre =
                request.Nombre.Trim(),

            Domicilio =
                request.Domicilio?.Trim(),

            Iva =
                request.Iva?.Trim(),

            Cuit =
                cuit,

            Habilitado =
                request.Habilitado
        };

        var id =
            await _plantaRepository
                .CrearAsync(planta);

        return Ok(new
        {
            ok = true,
            idPlanta = id,
            mensaje =
                "Planta creada correctamente."
        });
    }

    // -------------------------------------------------------
    // PUT /api/Plantas/{id}
    // -------------------------------------------------------

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(
        int id,
        PlantaRequest request)
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
            await _plantaRepository.ObtenerPorIdAsync(
                id,
                usuario.IdTranspor.Value);

        if (planta == null)
            return NotFound();

        var error =
            Validar(request);

        if (error != null)
            return BadRequest(error);

        var cuit =
            LimpiarCuit(request.Cuit);

        planta.Nombre =
            request.Nombre.Trim();

        planta.Domicilio =
            request.Domicilio?.Trim();

        planta.Iva =
            request.Iva?.Trim();

        planta.Cuit =
            cuit;

        planta.Habilitado =
            request.Habilitado;

        await _plantaRepository
            .ActualizarAsync(planta);

        return Ok(new
        {
            ok = true,
            mensaje =
                "Planta actualizada correctamente."
        });
    }

    // -------------------------------------------------------
    // DELETE /api/Plantas/{id}
    // BAJA LOGICA
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

        var planta =
            await _plantaRepository.ObtenerPorIdAsync(
                id,
                usuario.IdTranspor.Value);

        if (planta == null)
            return NotFound();

        await _plantaRepository.BajaAsync(
            id,
            usuario.IdTranspor.Value);

        return Ok(new
        {
            ok = true,
            mensaje =
                "Planta deshabilitada correctamente."
        });
    }

    // -------------------------------------------------------
    // VALIDACIONES
    // -------------------------------------------------------

    private static string? Validar(
        PlantaRequest request)
    {
        if (string.IsNullOrWhiteSpace(
            request.Nombre))
        {
            return "El nombre de la planta es obligatorio.";
        }

        if (request.Nombre.Trim().Length > 80)
        {
            return "El nombre no puede superar los 80 caracteres.";
        }

        if (request.Domicilio?.Trim().Length > 100)
        {
            return "El domicilio no puede superar los 100 caracteres.";
        }

        if (request.Iva?.Trim().Length > 1)
        {
            return "La condición de IVA no es válida.";
        }

        var cuit =
            LimpiarCuit(request.Cuit);

        if (cuit.Length != 11 ||
            !cuit.All(char.IsDigit))
        {
            return "El CUIT debe contener 11 dígitos.";
        }

        return null;
    }

    private static string LimpiarCuit(
        string? cuit)
    {
        return new string(
            (cuit ?? "")
            .Where(char.IsDigit)
            .ToArray());
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

    private static PlantaResponse MapearPlanta(
        Planta planta)
    {
        return new PlantaResponse
        {
            IdPlanta =
                planta.IdPlanta,

            Nombre =
                planta.Nombre,

            Domicilio =
                planta.Domicilio,

            Iva =
                planta.Iva,

            Cuit =
                planta.Cuit,

            Habilitado =
                planta.Habilitado
        };
    }
}