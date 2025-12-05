namespace CinteTestNet.Application.DTOs;

public class CompraDto
{
    public int Id { get; set; }
    public string NumeroFactura { get; set; } = string.Empty;
    public DateTime FechaCompra { get; set; }
    public decimal Monto { get; set; }
    public string? Descripcion { get; set; }
    public string Estado { get; set; } = string.Empty;
}

