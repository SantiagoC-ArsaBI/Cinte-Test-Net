using Microsoft.AspNetCore.Mvc;
using CinteTestNet.Application.Services;

namespace CinteTestNet.API.Controllers;

[ApiController]
[Route("api/tipos-documento")]
public class TiposDocumentoController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public TiposDocumentoController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CinteTestNet.Application.DTOs.TipoDocumentoDto>>> GetAll()
    {
        var tipos = await _clienteService.ObtenerTiposDocumentoAsync();
        return Ok(tipos);
    }
}

