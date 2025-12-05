namespace CinteTestNet.Domain.Entities;

public class EstadoCompra
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty; // completada, pendiente, cancelada
    public string Codigo { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    
    // Navegación
    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
}

