namespace CinteTestNet.Domain.Entities;

public class Cliente
{
    public int Id { get; set; }
    public int TipoDocumentoId { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;
    
    // Navegación
    public TipoDocumento TipoDocumento { get; set; } = null!;
    public ICollection<Compra> Compras { get; set; } = new List<Compra>();
}

