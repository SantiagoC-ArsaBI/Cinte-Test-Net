using Microsoft.EntityFrameworkCore;
using CinteTestNet.Application.Interfaces;
using CinteTestNet.Domain.Entities;
using CinteTestNet.Infrastructure.Data;

namespace CinteTestNet.Infrastructure.Repositories;

public class ClienteRepository : Repository<Cliente>, IClienteRepository
{
    public ClienteRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Cliente?> GetByDocumentoAsync(int tipoDocumentoId, string numeroDocumento)
    {
        // Limpiar espacios y normalizar el número de documento
        var numeroLimpio = numeroDocumento?.Trim() ?? string.Empty;
        
        return await _dbSet
            .Include(c => c.TipoDocumento)
            .Include(c => c.Compras)
                .ThenInclude(comp => comp.EstadoCompra)
            .FirstOrDefaultAsync(c => c.TipoDocumentoId == tipoDocumentoId 
                && c.NumeroDocumento.Trim() == numeroLimpio
                && c.Activo);
    }

    public async Task<Cliente?> GetByIdWithComprasAsync(int id)
    {
        return await _dbSet
            .Include(c => c.TipoDocumento)
            .Include(c => c.Compras)
                .ThenInclude(comp => comp.EstadoCompra)
            .FirstOrDefaultAsync(c => c.Id == id && c.Activo);
    }

    public async Task<IEnumerable<Cliente>> GetClientesFidelizablesAsync(DateTime fechaInicio, DateTime fechaFin, decimal montoMinimo)
    {
        var clientes = await _dbSet
            .Include(c => c.TipoDocumento)
            .Include(c => c.Compras)
                .ThenInclude(comp => comp.EstadoCompra)
            .Where(c => c.Activo 
                && c.Compras.Any(comp => 
                    comp.FechaCompra >= fechaInicio 
                    && comp.FechaCompra <= fechaFin 
                    && comp.EstadoCompra.Codigo == "completada"))
            .ToListAsync();

        return clientes.Where(c =>
        {
            var montoTotal = c.Compras
                .Where(comp => comp.FechaCompra >= fechaInicio 
                    && comp.FechaCompra <= fechaFin 
                    && comp.EstadoCompra.Codigo == "completada")
                .Sum(comp => comp.Monto);
            return montoTotal >= montoMinimo;
        });
    }
}

