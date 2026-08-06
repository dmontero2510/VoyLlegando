using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoyLlegando.Application.DTOs;
using VoyLlegando.Domain.Entities;
using VoyLlegando.Application.Interfaces;
using VoyLlegando.Infrastructure.Repositories;

namespace VoyLlegando.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]

public class UsuariosController : ControllerBase
{
	private readonly UsuarioRepository _repo;

	public UsuariosController(UsuarioRepository repo)
	{
    	_repo = repo;
	}

    //-------------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _repo.ObtenerTodosAsync());
    }

    //-------------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var u = await _repo.ObtenerPorIdAsync(id);

        if (u == null)
            return NotFound();

        return Ok(u);
    }

    //-------------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> Post(UsuarioRequest request)
    {
        if (await _repo.ExisteCelularAsync(request.Celular))
            return BadRequest("Ya existe ese celular.");

        var usuario = new Usuario
        {
            Celular = request.Celular,
            Clave = BCrypt.Net.BCrypt.HashPassword(request.Clave),
            Nombre = request.Nombre,
            Habilitado = true,
            Rol = request.Rol,

            IdTranspor = request.IdTranspor,

            Domicilio = request.Domicilio,
            Iva = request.Iva,
            Cuit = request.Cuit,
            Email = request.Email,

            IdPlanta = request.IdPlanta,
            IdProduc = request.IdProduc,

            PatChasis = request.PatChasis,
            PatAcopla = request.PatAcopla,

            Batea = request.Batea,
            Corta = request.Corta,
            Larga = request.Larga,
            Escala = request.Escala,

            Estado = request.Estado
        };

        var id = await _repo.CrearAsync(usuario);

        return Ok(new
        {
            ok = true,
            id
        });
    }

    //-------------------------------------------------------

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.BajaAsync(id);

        return Ok(new
        {
            ok = true
        });
    }
}