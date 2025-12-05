using CinteTestNet.Application.DTOs;

namespace CinteTestNet.Application.Services;

public interface IClienteService
{
    Task<ClienteDto?> BuscarClientePorDocumentoAsync(int tipoDocumentoId, string numeroDocumento);
    Task<IEnumerable<TipoDocumentoDto>> ObtenerTiposDocumentoAsync();
    Task<byte[]> ExportarClienteAsync(int clienteId, string formato);
    Task<byte[]> GenerarReporteFidelizacionAsync();
}

