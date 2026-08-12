using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Api.Controllers;

[ApiController]
[Authorize(Roles = "S")]
[Route("api/[controller]")]
public class LogisticasController : ControllerBase
{
    private readonly ILogisticaRepository _repo;

    public LogisticasController(
        ILogisticaRepository repo)
    {
        _repo = repo;
    }


    // -------------------------------------------------------
    // GET /api/Logisticas
    // -------------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var logisticas =
            await _repo.ObtenerTodosAsync();

        var respuesta =
            logisticas
                .OrderBy(x => x.Nombre)
                .Select(x => new
                {
                    idTranspor =
                        x.IdTranspor,

                    nombre =
                        x.Nombre,

                    domicilio =
                        x.Domicilio,

                    iva =
                        x.Iva,

                    cuit =
                        x.Cuit,

                    habilitado =
                        x.Habilitado
                });

        return Ok(respuesta);
    }


    // -------------------------------------------------------
    // POST /api/Logisticas
    // -------------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Post(
        LogisticaRequest request)
    {
        if (
            string.IsNullOrWhiteSpace(
                request.Nombre
            )
        )
        {
            return BadRequest(
                "Ingrese el nombre de la logística."
            );
        }


        if (
            string.IsNullOrWhiteSpace(
                request.Cuit
            )
        )
        {
            return BadRequest(
                "Ingrese el CUIT."
            );
        }


        var cuit =
            new string(
                request.Cuit
                    .Where(char.IsDigit)
                    .ToArray()
            );


        if (
            cuit.Length != 11
        )
        {
            return BadRequest(
                "El CUIT debe contener 11 dígitos."
            );
        }


        var logistica =
            new Logistica
            {
                Nombre =
                    request.Nombre.Trim(),

                Domicilio =
                    string.IsNullOrWhiteSpace(
                        request.Domicilio
                    )
                        ? null
                        : request.Domicilio.Trim(),

                Iva =
                    request.Iva,

                Cuit =
                    cuit,

                Habilitado =
                    true
            };


        var id =
            await _repo.CrearAsync(
                logistica
            );


        return Ok(new
        {
            ok = true,

            idTranspor =
                id,

            mensaje =
                "Logística creada correctamente."
        });
    }
}


// -------------------------------------------------------
// REQUEST
// -------------------------------------------------------

public class LogisticaRequest
{
    public string Nombre { get; set; } =
        "";

    public string? Domicilio { get; set; }

    public string? Iva { get; set; }

    public string Cuit { get; set; } =
        "";
}