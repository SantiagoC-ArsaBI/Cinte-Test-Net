namespace CinteTestNet.Application.DTOs;

public class ClienteDto
{
    public int Id { get; set; }
    public TipoDocumentoDto TipoDocumento { get; set; } = null!;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public List<CompraDto> Compras { get; set; } = new();
    public int TotalCompras { get; set; }
    public decimal MontoTotalCompras { get; set; }
}

