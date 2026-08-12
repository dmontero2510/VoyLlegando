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
public class CamposController : ControllerBase
{
    private readonly ICampoRepository _campoRepository;
    private readonly IProductorRepository _productorRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public CamposController(
        ICampoRepository campoRepository,
        IProductorRepository productorRepository,
        IUsuarioRepository usuarioRepository)
    {
        _campoRepository = campoRepository;
        _productorRepository = productorRepository;
        _usuarioRepository = usuarioRepository;
    }

    // -------------------------------------------------------
    // GET /api/Campos/productor/{idProductor}
    // -------------------------------------------------------

    [HttpGet("productor/{idProductor:int}")]
    public async Task<IActionResult> PorProductor(
        int idProductor)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
            return BadRequest(
                "La logística no tiene una empresa asociada.");

        var productor =
            await _productorRepository
                .ObtenerPorIdAsync(
                    idProductor,
                    usuario.IdTranspor.Value);

        if (productor == null)
            return NotFound(
                "El productor no existe o no pertenece a esta logística.");

        var campos =
            await _campoRepository
                .ObtenerPorProductorAsync(idProductor);

        return Ok(
            campos.Select(MapearCampo));
    }

    // -------------------------------------------------------
    // GET /api/Campos/{id}
    // -------------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
            return BadRequest(
                "La logística no tiene una empresa asociada.");

        var campo =
            await _campoRepository
                .ObtenerPorIdAsync(id);

        if (campo == null)
            return NotFound();

        var productor =
            await _productorRepository
                .ObtenerPorIdAsync(
                    campo.IdProductor,
                    usuario.IdTranspor.Value);

        if (productor == null)
            return NotFound();

        return Ok(
            MapearCampo(campo));
    }

    // -------------------------------------------------------
    // POST /api/Campos
    // -------------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Post(
        CampoRequest request)
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

        var productor =
            await _productorRepository
                .ObtenerPorIdAsync(
                    request.IdProductor,
                    usuario.IdTranspor.Value);

        if (productor == null)
            return BadRequest(
                "El productor no existe o no pertenece a esta logística.");

        if (!productor.Habilitado)
            return BadRequest(
                "El productor está deshabilitado.");

        Campo campo = new()
        {
            IdProductor =
                request.IdProductor,

            DescripCampo =
                request.DescripCampo.Trim(),

            Latitud =
                request.Latitud,

            Longitud =
                request.Longitud
        };

        var id =
            await _campoRepository
                .CrearAsync(campo);

        return Ok(new
        {
            ok = true,
            idCampo = id,
            mensaje =
                "Campo creado correctamente."
        });
    }

    // -------------------------------------------------------
    // PUT /api/Campos/{id}
    // -------------------------------------------------------

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(
        int id,
        CampoRequest request)
    {
        var usuario = await ObtenerUsuarioActual();

        if (usuario == null)
            return Unauthorized();

        if (usuario.Rol != "L")
            return Forbid();

        if (usuario.IdTranspor == null)
            return BadRequest(
                "La logística no tiene una empresa asociada.");

        var campo =
            await _campoRepository
                .ObtenerPorIdAsync(id);

        if (campo == null)
            return NotFound();

        // Validamos que el campo actual pertenezca
        // a un productor de esta logística.
        var productorActual =
            await _productorRepository
                .ObtenerPorIdAsync(
                    campo.IdProductor,
                    usuario.IdTranspor.Value);

        if (productorActual == null)
            return NotFound();

        var error =
            Validar(request);

        if (error != null)
            return BadRequest(error);

        // Validamos también el productor destino
        // por si se intenta cambiar el campo de productor.
        var productorNuevo =
            await _productorRepository
                .ObtenerPorIdAsync(
                    request.IdProductor,
                    usuario.IdTranspor.Value);

        if (productorNuevo == null)
            return BadRequest(
                "El productor no existe o no pertenece a esta logística.");

        if (!productorNuevo.Habilitado)
            return BadRequest(
                "El productor está deshabilitado.");

        campo.IdProductor =
            request.IdProductor;

        campo.DescripCampo =
            request.DescripCampo.Trim();

        campo.Latitud =
            request.Latitud;

        campo.Longitud =
            request.Longitud;

        await _campoRepository
            .ActualizarAsync(campo);

        return Ok(new
        {
            ok = true,
            mensaje =
                "Campo actualizado correctamente."
        });
    }

    // -------------------------------------------------------
    // DELETE /api/Campos/{id}
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

        var campo =
            await _campoRepository
                .ObtenerPorIdAsync(id);

        if (campo == null)
            return NotFound();

        var productor =
            await _productorRepository
                .ObtenerPorIdAsync(
                    campo.IdProductor,
                    usuario.IdTranspor.Value);

        if (productor == null)
            return NotFound();

        await _campoRepository
            .EliminarAsync(id);

        return Ok(new
        {
            ok = true,
            mensaje =
                "Campo eliminado correctamente."
        });
    }

    // -------------------------------------------------------
    // VALIDACIONES
    // -------------------------------------------------------

    private static string? Validar(
        CampoRequest request)
    {
        if (request.IdProductor <= 0)
            return "El productor es obligatorio.";

        if (string.IsNullOrWhiteSpace(
            request.DescripCampo))
        {
            return "La descripción del campo es obligatoria.";
        }

        if (request.DescripCampo.Trim().Length > 30)
        {
            return "La descripción del campo no puede superar los 30 caracteres.";
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

    private static CampoResponse MapearCampo(
        Campo campo)
    {
        return new CampoResponse
        {
            IdCampo =
                campo.IdCampo,

            IdProductor =
                campo.IdProductor,

            DescripCampo =
                campo.DescripCampo,

            Latitud =
                campo.Latitud,

            Longitud =
                campo.Longitud
        };
    }
}