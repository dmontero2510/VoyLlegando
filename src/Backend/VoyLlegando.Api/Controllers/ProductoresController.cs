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
public class ProductoresController : ControllerBase
{
    private readonly IProductorRepository _productorRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public ProductoresController(
        IProductorRepository productorRepository,
        IUsuarioRepository usuarioRepository)
    {
        _productorRepository = productorRepository;
        _usuarioRepository = usuarioRepository;
    }

    // -------------------------------------------------------
    // GET /api/Productores
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

        var productores =
            await _productorRepository.ObtenerTodosAsync(
                usuario.IdTranspor.Value);

        return Ok(
            productores.Select(MapearProductor));
    }

    // -------------------------------------------------------
    // GET /api/Productores/{id}
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

        var productor =
            await _productorRepository.ObtenerPorIdAsync(
                id,
                usuario.IdTranspor.Value);

        if (productor == null)
            return NotFound();

        return Ok(
            MapearProductor(productor));
    }

    // -------------------------------------------------------
    // POST /api/Productores
    // -------------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Post(
        ProductorRequest request)
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

        Productor productor = new()
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
            await _productorRepository
                .CrearAsync(productor);

        return Ok(new
        {
            ok = true,
            idProductor = id,
            mensaje =
                "Productor creado correctamente."
        });
    }

    // -------------------------------------------------------
    // PUT /api/Productores/{id}
    // -------------------------------------------------------

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(
        int id,
        ProductorRequest request)
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
            await _productorRepository.ObtenerPorIdAsync(
                id,
                usuario.IdTranspor.Value);

        if (productor == null)
            return NotFound();

        var error =
            Validar(request);

        if (error != null)
            return BadRequest(error);

        var cuit =
            LimpiarCuit(request.Cuit);

        productor.Nombre =
            request.Nombre.Trim();

        productor.Domicilio =
            request.Domicilio?.Trim();

        productor.Iva =
            request.Iva?.Trim();

        productor.Cuit =
            cuit;

        productor.Habilitado =
            request.Habilitado;

        await _productorRepository
            .ActualizarAsync(productor);

        return Ok(new
        {
            ok = true,
            mensaje =
                "Productor actualizado correctamente."
        });
    }

    // -------------------------------------------------------
    // DELETE /api/Productores/{id}
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

        var productor =
            await _productorRepository.ObtenerPorIdAsync(
                id,
                usuario.IdTranspor.Value);

        if (productor == null)
            return NotFound();

        await _productorRepository.BajaAsync(
            id,
            usuario.IdTranspor.Value);

        return Ok(new
        {
            ok = true,
            mensaje =
                "Productor deshabilitado correctamente."
        });
    }

    // -------------------------------------------------------
    // VALIDACIONES
    // -------------------------------------------------------

    private static string? Validar(
        ProductorRequest request)
    {
        if (string.IsNullOrWhiteSpace(
            request.Nombre))
        {
            return "El nombre del productor es obligatorio.";
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

    private static ProductorResponse MapearProductor(
        Productor productor)
    {
        return new ProductorResponse
        {
            IdProductor =
                productor.IdProductor,

            Nombre =
                productor.Nombre,

            Domicilio =
                productor.Domicilio,

            Iva =
                productor.Iva,

            Cuit =
                productor.Cuit,

            Habilitado =
                productor.Habilitado
        };
    }
}