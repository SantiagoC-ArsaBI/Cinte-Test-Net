using Microsoft.AspNetCore.Mvc;
using CinteTestNet.Application.Services;

namespace CinteTestNet.API.Controllers;

[ApiController]
[Route("api/reporte-fidelizacion")]
public class ReporteFidelizacionController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ReporteFidelizacionController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet("generar")]
    public async Task<IActionResult> Generar()
    {
        try
        {
            var archivo = await _clienteService.GenerarReporteFidelizacionAsync();
            var fecha = DateTime.UtcNow.ToString("yyyy-MM-dd");
            return File(archivo, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                $"reporte_fidelizacion_{fecha}.xlsx");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error al generar el reporte", detalles = ex.Message });
        }
    }
}

