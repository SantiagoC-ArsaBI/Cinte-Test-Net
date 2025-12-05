using Microsoft.AspNetCore.Mvc;
using CinteTestNet.Application.Services;

namespace CinteTestNet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet("buscar")]
    public async Task<ActionResult<CinteTestNet.Application.DTOs.ClienteDto>> Buscar(
        [FromQuery] int tipo_documento_id,
        [FromQuery] string numero_documento)
    {
        if (string.IsNullOrWhiteSpace(numero_documento))
        {
            return BadRequest(new { error = "El número de documento es requerido" });
        }

        var cliente = await _clienteService.BuscarClientePorDocumentoAsync(tipo_documento_id, numero_documento.Trim());

        if (cliente == null)
        {
            return NotFound(new { error = "Cliente no encontrado" });
        }

        return Ok(cliente);
    }

    [HttpGet("{id}/exportar")]
    public async Task<IActionResult> Exportar(int id, [FromQuery] string formato = "excel")
    {
        try
        {
            var archivo = await _clienteService.ExportarClienteAsync(id, formato);

            var contentType = formato.ToLower() switch
            {
                "csv" => "text/csv",
                "excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "txt" => "text/plain",
                _ => "application/octet-stream"
            };

            var extension = formato.ToLower() switch
            {
                "csv" => "csv",
                "excel" => "xlsx",
                "txt" => "txt",
                _ => "bin"
            };

            return File(archivo, contentType, $"cliente_{id}.{extension}");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

