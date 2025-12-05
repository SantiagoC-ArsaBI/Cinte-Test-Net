namespace CinteTestNet.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IClienteRepository Clientes { get; }
    IRepository<CinteTestNet.Domain.Entities.TipoDocumento> TiposDocumento { get; }
    IRepository<CinteTestNet.Domain.Entities.Compra> Compras { get; }
    IRepository<CinteTestNet.Domain.Entities.EstadoCompra> EstadosCompra { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

