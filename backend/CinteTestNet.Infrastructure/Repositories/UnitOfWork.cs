using Microsoft.EntityFrameworkCore.Storage;
using CinteTestNet.Application.Interfaces;
using CinteTestNet.Infrastructure.Data;

namespace CinteTestNet.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IClienteRepository? _clientes;
    private IRepository<CinteTestNet.Domain.Entities.TipoDocumento>? _tiposDocumento;
    private IRepository<CinteTestNet.Domain.Entities.Compra>? _compras;
    private IRepository<CinteTestNet.Domain.Entities.EstadoCompra>? _estadosCompra;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IClienteRepository Clientes
    {
        get
        {
            _clientes ??= new ClienteRepository(_context);
            return _clientes;
        }
    }

    public IRepository<CinteTestNet.Domain.Entities.TipoDocumento> TiposDocumento
    {
        get
        {
            _tiposDocumento ??= new Repository<CinteTestNet.Domain.Entities.TipoDocumento>(_context);
            return _tiposDocumento;
        }
    }

    public IRepository<CinteTestNet.Domain.Entities.Compra> Compras
    {
        get
        {
            _compras ??= new Repository<CinteTestNet.Domain.Entities.Compra>(_context);
            return _compras;
        }
    }

    public IRepository<CinteTestNet.Domain.Entities.EstadoCompra> EstadosCompra
    {
        get
        {
            _estadosCompra ??= new Repository<CinteTestNet.Domain.Entities.EstadoCompra>(_context);
            return _estadosCompra;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

