using Microsoft.EntityFrameworkCore;
using CinteTestNet.Domain.Entities;

namespace CinteTestNet.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TipoDocumento> TiposDocumento { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<EstadoCompra> EstadosCompra { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de TipoDocumento
        modelBuilder.Entity<TipoDocumento>(entity =>
        {
            entity.ToTable("TiposDocumento");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
            entity.HasIndex(e => e.Codigo).IsUnique();
        });

        // Configuración de Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroDocumento).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Correo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.FechaRegistro).IsRequired();

            entity.HasIndex(e => new { e.TipoDocumentoId, e.NumeroDocumento }).IsUnique();
            
            entity.HasOne(e => e.TipoDocumento)
                .WithMany(t => t.Clientes)
                .HasForeignKey(e => e.TipoDocumentoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de EstadoCompra
        modelBuilder.Entity<EstadoCompra>(entity =>
        {
            entity.ToTable("EstadosCompra");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.Codigo).IsUnique();
        });

        // Configuración de Compra
        modelBuilder.Entity<Compra>(entity =>
        {
            entity.ToTable("Compras");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroFactura).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FechaCompra).IsRequired();
            entity.Property(e => e.Monto).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.FechaCreacion).IsRequired();

            entity.HasIndex(e => e.NumeroFactura).IsUnique();
            
            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Compras)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.EstadoCompra)
                .WithMany(ec => ec.Compras)
                .HasForeignKey(e => e.EstadoCompraId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

