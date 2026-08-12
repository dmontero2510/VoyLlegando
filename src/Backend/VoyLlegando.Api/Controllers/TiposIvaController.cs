using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoyLlegando.Application.Interfaces;

namespace VoyLlegando.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TiposIvaController : ControllerBase
{
    private readonly ITipoIvaRepository _repository;

    public TiposIvaController(
        ITipoIvaRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var tipos =
            await _repository.ObtenerTodosAsync();

        return Ok(
            tipos.Select(x => new
            {
                idIva = x.IdIva,
                descripcion = x.DescripIva
            }));
    }
}