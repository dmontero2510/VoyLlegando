using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ViajeDocumentosController : ControllerBase
{
    private readonly IViajeDocumentoRepository _documentoRepository;
    private readonly IViajeRepository _viajeRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    private const long TamanoMaximoPdf =
        2 * 1024 * 1024; // 2 MB


    public ViajeDocumentosController(
        IViajeDocumentoRepository documentoRepository,
        IViajeRepository viajeRepository,
        IUsuarioRepository usuarioRepository)
    {
        _documentoRepository =
            documentoRepository;

        _viajeRepository =
            viajeRepository;

        _usuarioRepository =
            usuarioRepository;
    }


    // =========================================================
    // SUBIR / REEMPLAZAR DOCUMENTO
    //
    // SOLO LOGISTICA (L)
    // =========================================================

    [HttpPost("{idViaje:int}")]
    public async Task<IActionResult> Subir(
        int idViaje,
        IFormFile archivo,
        [FromForm] string tipo)
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


        // -----------------------------------------------------
        // SOLO LOGISTICA PUEDE ADJUNTAR / REEMPLAZAR
        // -----------------------------------------------------

        if (usuario.Rol != "L")
            return Forbid();


        var viaje =
            await _viajeRepository
                .ObtenerPorIdAsync(idViaje);


        if (viaje == null)
            return NotFound(
                "El viaje no existe."
            );


        // -----------------------------------------------------
        // LA LOGISTICA SOLO PUEDE MODIFICAR SUS PROPIOS VIAJES
        // -----------------------------------------------------

        if (!PuedeAdministrarViaje(
                usuario,
                viaje))
        {
            return Forbid();
        }


        tipo =
            (tipo ?? "")
                .Trim()
                .ToUpperInvariant();


        if (string.IsNullOrWhiteSpace(tipo))
        {
            return BadRequest(
                "Debe indicar el tipo de documento."
            );
        }


        if (tipo.Length > 10)
        {
            return BadRequest(
                "El tipo de documento no puede superar los 10 caracteres."
            );
        }


        if (archivo == null ||
            archivo.Length == 0)
        {
            return BadRequest(
                "Debe seleccionar un archivo."
            );
        }


        if (archivo.Length > TamanoMaximoPdf)
        {
            return BadRequest(
                "El archivo no puede superar los 2 MB."
            );
        }


        var extension =
            Path.GetExtension(
                archivo.FileName
            );


        if (!string.Equals(
                extension,
                ".pdf",
                StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(
                "Solamente se permiten archivos PDF."
            );
        }


        // -----------------------------------------------------
        // LEER CONTENIDO
        // -----------------------------------------------------

        byte[] contenido;


        await using (
            var memoria =
                new MemoryStream())
        {
            await archivo
                .CopyToAsync(memoria);

            contenido =
                memoria.ToArray();
        }


        // -----------------------------------------------------
        // VALIDAR FIRMA PDF
        // -----------------------------------------------------

        if (!EsPdf(contenido))
        {
            return BadRequest(
                "El archivo seleccionado no es un PDF válido."
            );
        }


        // -----------------------------------------------------
        // CREAR / REEMPLAZAR DOCUMENTO
        //
        // Para CP, el Repository ya garantiza:
        // 1 viaje = 1 CP
        // -----------------------------------------------------

        var documento =
            new ViajeDocumento
            {
                IdViaje =
                    idViaje,

                Tipo =
                    tipo,

                NombreArchivo =
                    Path.GetFileName(
                        archivo.FileName
                    ),

                Contenido =
                    contenido
            };


        var idDocumento =
            await _documentoRepository
                .CrearAsync(documento);


        return Ok(
            new
            {
                ok = true,

                idDocumento,

                mensaje =
                    "Documento guardado correctamente."
            }
        );
    }


    // =========================================================
    // LISTAR DOCUMENTOS DE UN VIAJE
    //
    // L = SUS VIAJES
    // E = VIAJE ASIGNADO
    // S = TODOS
    // =========================================================

    [HttpGet("viaje/{idViaje:int}")]
    public async Task<IActionResult> ObtenerPorViaje(
        int idViaje)
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


        var viaje =
            await _viajeRepository
                .ObtenerPorIdAsync(idViaje);


        if (viaje == null)
        {
            return NotFound(
                "El viaje no existe."
            );
        }


        if (!PuedeAccederAlViaje(
                usuario,
                viaje))
        {
            return Forbid();
        }


        var documentos =
            await _documentoRepository
                .ObtenerPorViajeAsync(idViaje);


        // -----------------------------------------------------
        // NO ENVIAMOS EL BYTE[] EN EL LISTADO
        // -----------------------------------------------------

        var resultado =
            documentos.Select(
                d => new
                {
                    d.IdDocumento,
                    d.IdViaje,
                    d.Tipo,
                    d.NombreArchivo,
                    d.Fecha
                }
            );


        return Ok(resultado);
    }


    // =========================================================
    // VER / DESCARGAR DOCUMENTO
    //
    // L = SUS VIAJES
    // E = VIAJE ASIGNADO
    // S = TODOS
    // =========================================================

    [HttpGet("{idDocumento:int}")]
    public async Task<IActionResult> Obtener(
        int idDocumento)
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


        var documento =
            await _documentoRepository
                .ObtenerPorIdAsync(
                    idDocumento
                );


        if (documento == null)
        {
            return NotFound(
                "El documento no existe."
            );
        }


        var viaje =
            await _viajeRepository
                .ObtenerPorIdAsync(
                    documento.IdViaje
                );


        if (viaje == null)
        {
            return NotFound(
                "El viaje no existe."
            );
        }


        if (!PuedeAccederAlViaje(
                usuario,
                viaje))
        {
            return Forbid();
        }


        var nombreArchivo =
            string.IsNullOrWhiteSpace(
                documento.NombreArchivo)
                ? $"documento_{documento.IdDocumento}.pdf"
                : documento.NombreArchivo;


        return File(
            documento.Contenido,
            "application/pdf",
            nombreArchivo
        );
    }


    // =========================================================
    // ELIMINAR DOCUMENTO
    //
    // SOLO LOGISTICA (L)
    // =========================================================

    [HttpDelete("{idDocumento:int}")]
    public async Task<IActionResult> Eliminar(
        int idDocumento)
    {
        var usuario =
            await ObtenerUsuarioActual();


        if (usuario == null)
            return Unauthorized();


        // -----------------------------------------------------
        // SOLO LOGISTICA PUEDE QUITAR DOCUMENTOS
        // -----------------------------------------------------

        if (usuario.Rol != "L")
            return Forbid();


        var documento =
            await _documentoRepository
                .ObtenerPorIdAsync(
                    idDocumento
                );


        if (documento == null)
        {
            return NotFound(
                "El documento no existe."
            );
        }


        var viaje =
            await _viajeRepository
                .ObtenerPorIdAsync(
                    documento.IdViaje
                );


        if (viaje == null)
        {
            return NotFound(
                "El viaje no existe."
            );
        }


        // -----------------------------------------------------
        // SOLO PUEDE ELIMINAR DOCUMENTOS DE SUS PROPIOS VIAJES
        // -----------------------------------------------------

        if (!PuedeAdministrarViaje(
                usuario,
                viaje))
        {
            return Forbid();
        }


        await _documentoRepository
            .EliminarAsync(
                idDocumento
            );


        return Ok(
            new
            {
                ok = true,

                mensaje =
                    "Documento eliminado correctamente."
            }
        );
    }


    // =========================================================
    // SEGURIDAD
    // =========================================================

    private static bool PuedeAccederAlViaje(
        Usuario usuario,
        Viaje viaje)
    {
        // -----------------------------------------------------
        // ADMINISTRADOR
        // -----------------------------------------------------

        if (usuario.Rol == "S")
            return true;


        // -----------------------------------------------------
        // LOGISTICA
        // -----------------------------------------------------

        if (usuario.Rol == "L")
        {
            return
                usuario.IdTranspor.HasValue &&
                viaje.IdTranspor ==
                usuario.IdTranspor.Value;
        }


        // -----------------------------------------------------
        // EMPRESA / CAMIONERO
        // SOLO VIAJE ASIGNADO A ESE USUARIO
        // -----------------------------------------------------

        if (usuario.Rol == "E")
        {
            return
                viaje.IdCamionero ==
                usuario.IdUsuario;
        }


        return false;
    }


    // =========================================================
    // SEGURIDAD PARA MODIFICAR DOCUMENTOS
    //
    // EXCLUSIVAMENTE LOGISTICA
    // =========================================================

    private static bool PuedeAdministrarViaje(
        Usuario usuario,
        Viaje viaje)
    {
        if (usuario.Rol != "L")
            return false;


        return
            usuario.IdTranspor.HasValue &&
            viaje.IdTranspor ==
            usuario.IdTranspor.Value;
    }


    // =========================================================
    // OBTENER USUARIO ACTUAL
    // =========================================================

    private async Task<Usuario?>
        ObtenerUsuarioActual()
    {
        var idClaim =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );


        if (!int.TryParse(
                idClaim,
                out var idUsuario))
        {
            return null;
        }


        return await _usuarioRepository
            .ObtenerPorIdAsync(
                idUsuario
            );
    }


    // =========================================================
    // VALIDACION PDF
    // =========================================================

    private static bool EsPdf(
        byte[] contenido)
    {
        if (contenido.Length < 5)
            return false;


        // Todo PDF comienza con "%PDF-"

        return
            contenido[0] == 0x25 && // %
            contenido[1] == 0x50 && // P
            contenido[2] == 0x44 && // D
            contenido[3] == 0x46 && // F
            contenido[4] == 0x2D;   // -
    }
}