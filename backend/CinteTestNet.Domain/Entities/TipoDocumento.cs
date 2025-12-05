namespace CinteTestNet.Domain.Entities;

public class TipoDocumento
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty; // NIT, CC, PA
    public bool Activo { get; set; } = true;
    
    // Navegación
    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}

