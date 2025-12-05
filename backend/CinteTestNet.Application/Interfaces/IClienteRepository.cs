using CinteTestNet.Domain.Entities;

namespace CinteTestNet.Application.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> GetByDocumentoAsync(int tipoDocumentoId, string numeroDocumento);
    Task<Cliente?> GetByIdWithComprasAsync(int id);
    Task<IEnumerable<Cliente>> GetClientesFidelizablesAsync(DateTime fechaInicio, DateTime fechaFin, decimal montoMinimo);
}

