using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoyLlegando.Application.DTOs;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Domain.Entities;

namespace VoyLlegando.Api.Controllers;


[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CerealesController
    : ControllerBase
{
    private readonly ICerealRepository
        _cerealRepository;


    // -------------------------------------------------------
    // CONSTRUCTOR
    // -------------------------------------------------------

    public CerealesController(
        ICerealRepository cerealRepository)
    {
        _cerealRepository =
            cerealRepository;
    }


    // -------------------------------------------------------
    // GET /api/Cereales
    //
    // SOLO S
    // LISTADO COMPLETO PARA ADMINISTRACION
    // -------------------------------------------------------

    [Authorize(Roles = "S")]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
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
    //
    // L = LOGISTICA
    // S = ADMINISTRADOR
    //
    // PARA COMBOS
    // -------------------------------------------------------

    [Authorize(Roles = "L,S")]
    [HttpGet("habilitados")]
    public async Task<IActionResult>
        Habilitados()
    {
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
    //
    // SOLO S
    // -------------------------------------------------------

    [Authorize(Roles = "S")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(
        int id)
    {
        var cereal =
            await _cerealRepository
                .ObtenerPorIdAsync(
                    id
                );


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
    //
    // SOLO S
    // -------------------------------------------------------

    [Authorize(Roles = "S")]
    [HttpPost]
    public async Task<IActionResult> Post(
        CerealRequest request)
    {
        if (
            request.IdCereal <= 0
        )
        {
            return BadRequest(
                "El código del cereal es obligatorio."
            );
        }


        if (
            string.IsNullOrWhiteSpace(
                request.Nombre
            )
        )
        {
            return BadRequest(
                "El nombre del cereal es obligatorio."
            );
        }


        var existente =
            await _cerealRepository
                .ObtenerPorIdAsync(
                    request.IdCereal
                );


        if (existente != null)
        {
            return BadRequest(
                "Ya existe un cereal con ese código."
            );
        }


        Cereal cereal =
            new()
            {
                IdCereal =
                    request.IdCereal,

                NombreCereal =
                    request.Nombre
                        .Trim(),

                Habilitado =
                    true
            };


        await _cerealRepository
            .CrearAsync(
                cereal
            );


        return Ok(new
        {
            ok =
                true,

            mensaje =
                "Cereal creado correctamente."
        });
    }


    // -------------------------------------------------------
    // PUT /api/Cereales/{id}
    //
    // SOLO S
    //
    // PERMITE MODIFICAR Y TAMBIEN REHABILITAR
    // -------------------------------------------------------

    [Authorize(Roles = "S")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(
        int id,
        CerealRequest request)
    {
        var cereal =
            await _cerealRepository
                .ObtenerPorIdAsync(
                    id
                );


        if (cereal == null)
            return NotFound();


        if (
            string.IsNullOrWhiteSpace(
                request.Nombre
            )
        )
        {
            return BadRequest(
                "El nombre del cereal es obligatorio."
            );
        }


        cereal.NombreCereal =
            request.Nombre
                .Trim();


        cereal.Habilitado =
            request.Habilitado;


        await _cerealRepository
            .ActualizarAsync(
                cereal
            );


        return Ok(new
        {
            ok =
                true,

            mensaje =
                cereal.Habilitado
                    ? "Cereal actualizado correctamente."
                    : "Cereal actualizado y deshabilitado correctamente."
        });
    }


    // -------------------------------------------------------
    // DELETE /api/Cereales/{id}
    //
    // SOLO S
    //
    // BAJA LOGICA
    // -------------------------------------------------------

    [Authorize(Roles = "S")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var cereal =
            await _cerealRepository
                .ObtenerPorIdAsync(
                    id
                );


        if (cereal == null)
            return NotFound();


        if (!cereal.Habilitado)
        {
            return Ok(new
            {
                ok =
                    true,

                mensaje =
                    "El cereal ya se encuentra deshabilitado."
            });
        }


        await _cerealRepository
            .BajaAsync(
                id
            );


        return Ok(new
        {
            ok =
                true,

            mensaje =
                "Cereal deshabilitado correctamente."
        });
    }
}