namespace CinteTestNet.Domain.Entities;

public class Compra
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string NumeroFactura { get; set; } = string.Empty;
    public DateTime FechaCompra { get; set; }
    public decimal Monto { get; set; }
    public string? Descripcion { get; set; }
    public int EstadoCompraId { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    // Navegación
    public Cliente Cliente { get; set; } = null!;
    public EstadoCompra EstadoCompra { get; set; } = null!;
}

