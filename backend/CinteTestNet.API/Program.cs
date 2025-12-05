using Microsoft.EntityFrameworkCore;
using CinteTestNet.Infrastructure.Data;
using CinteTestNet.Infrastructure.Repositories;
using CinteTestNet.Application.Services;
using CinteTestNet.Application.Interfaces;
using CinteTestNet.Domain.Entities;
using CinteTestNet.API;

var builder = WebApplication.CreateBuilder(args);

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// Configurar Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=cinte_test.db"));

// Configurar inyección de dependencias
builder.Services.AddScoped<CinteTestNet.Application.Interfaces.IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IClienteService, ClienteService>();

// Configurar controladores con serialización snake_case
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
    });

// Configurar OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS debe ir antes de UseHttpsRedirection
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Crear base de datos y datos de prueba
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        context.Database.EnsureCreated();
        logger.LogInformation("Base de datos creada/verificada");
        
        SeedData(context);
        logger.LogInformation("Datos de prueba creados exitosamente");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar la base de datos");
    }
}

app.Run();

static void SeedData(ApplicationDbContext context)
{
    // Limpiar datos existentes para recrearlos
    if (context.Compras.Any())
    {
        context.Compras.RemoveRange(context.Compras);
        context.SaveChanges();
    }
    
    if (context.Clientes.Any())
    {
        context.Clientes.RemoveRange(context.Clientes);
        context.SaveChanges();
    }
    
    if (context.EstadosCompra.Any())
    {
        context.EstadosCompra.RemoveRange(context.EstadosCompra);
        context.SaveChanges();
    }
    
    if (context.TiposDocumento.Any())
    {
        context.TiposDocumento.RemoveRange(context.TiposDocumento);
        context.SaveChanges();
    }

    // Tipos de Documento
    var tipoNit = new TipoDocumento { Nombre = "NIT", Codigo = "NIT", Activo = true };
    var tipoCedula = new TipoDocumento { Nombre = "Cédula", Codigo = "CC", Activo = true };
    var tipoPasaporte = new TipoDocumento { Nombre = "Pasaporte", Codigo = "PA", Activo = true };
    
    context.TiposDocumento.AddRange(tipoNit, tipoCedula, tipoPasaporte);
    context.SaveChanges();

    // Estados de Compra
    var estadoCompletada = new EstadoCompra { Nombre = "completada", Codigo = "completada", Activo = true };
    var estadoPendiente = new EstadoCompra { Nombre = "pendiente", Codigo = "pendiente", Activo = true };
    var estadoCancelada = new EstadoCompra { Nombre = "cancelada", Codigo = "cancelada", Activo = true };
    
    context.EstadosCompra.AddRange(estadoCompletada, estadoPendiente, estadoCancelada);
    context.SaveChanges();

    // Clientes de prueba - 10 clientes variados (solo 3 fidelizables)
    var clientes = new List<Cliente>
    {
        // Cliente 1 - FIDELIZABLE (más de 5M)
        new Cliente
        {
            TipoDocumentoId = tipoCedula.Id,
            NumeroDocumento = "1234567890",
            Nombre = "Juan",
            Apellido = "Pérez",
            Correo = "juan.perez@example.com",
            Telefono = "3001234567",
            FechaRegistro = DateTime.UtcNow.AddMonths(-6),
            Activo = true
        },
        // Cliente 2 - NO FIDELIZABLE
        new Cliente
        {
            TipoDocumentoId = tipoCedula.Id,
            NumeroDocumento = "9876543210",
            Nombre = "María",
            Apellido = "González",
            Correo = "maria.gonzalez@example.com",
            Telefono = "3009876543",
            FechaRegistro = DateTime.UtcNow.AddMonths(-3),
            Activo = true
        },
        // Cliente 3 - FIDELIZABLE (más de 5M) - NIT
        new Cliente
        {
            TipoDocumentoId = tipoNit.Id,
            NumeroDocumento = "900123456-1",
            Nombre = "Empresa",
            Apellido = "Ejemplo S.A.S.",
            Correo = "contacto@empresa.com",
            Telefono = "6012345678",
            FechaRegistro = DateTime.UtcNow.AddMonths(-12),
            Activo = true
        },
        // Cliente 4 - NO FIDELIZABLE
        new Cliente
        {
            TipoDocumentoId = tipoCedula.Id,
            NumeroDocumento = "1122334455",
            Nombre = "Carlos",
            Apellido = "Rodríguez",
            Correo = "carlos.rodriguez@example.com",
            Telefono = "3105551234",
            FechaRegistro = DateTime.UtcNow.AddMonths(-8),
            Activo = true
        },
        // Cliente 5 - NO FIDELIZABLE
        new Cliente
        {
            TipoDocumentoId = tipoCedula.Id,
            NumeroDocumento = "5566778899",
            Nombre = "Ana",
            Apellido = "Martínez",
            Correo = "ana.martinez@example.com",
            Telefono = "3206667890",
            FechaRegistro = DateTime.UtcNow.AddMonths(-4),
            Activo = true
        },
        // Cliente 6 - NO FIDELIZABLE - Pasaporte
        new Cliente
        {
            TipoDocumentoId = tipoPasaporte.Id,
            NumeroDocumento = "AB123456",
            Nombre = "Roberto",
            Apellido = "Silva",
            Correo = "roberto.silva@example.com",
            Telefono = "3157778901",
            FechaRegistro = DateTime.UtcNow.AddMonths(-10),
            Activo = true
        },
        // Cliente 7 - NO FIDELIZABLE - NIT
        new Cliente
        {
            TipoDocumentoId = tipoNit.Id,
            NumeroDocumento = "900987654-2",
            Nombre = "Distribuidora",
            Apellido = "Norte S.A.",
            Correo = "ventas@distribuidoranorte.com",
            Telefono = "6019876543",
            FechaRegistro = DateTime.UtcNow.AddMonths(-18),
            Activo = true
        },
        // Cliente 8 - NO FIDELIZABLE
        new Cliente
        {
            TipoDocumentoId = tipoCedula.Id,
            NumeroDocumento = "2233445566",
            Nombre = "Laura",
            Apellido = "Fernández",
            Correo = "laura.fernandez@example.com",
            Telefono = "3008889012",
            FechaRegistro = DateTime.UtcNow.AddMonths(-2),
            Activo = true
        },
        // Cliente 9 - NO FIDELIZABLE
        new Cliente
        {
            TipoDocumentoId = tipoCedula.Id,
            NumeroDocumento = "7788990011",
            Nombre = "Diego",
            Apellido = "López",
            Correo = "diego.lopez@example.com",
            Telefono = "3119990123",
            FechaRegistro = DateTime.UtcNow.AddMonths(-15),
            Activo = true
        },
        // Cliente 10 - FIDELIZABLE (más de 5M) - NIT
        new Cliente
        {
            TipoDocumentoId = tipoNit.Id,
            NumeroDocumento = "800111222-3",
            Nombre = "Comercializadora",
            Apellido = "Sur Ltda.",
            Correo = "info@comercializadorasur.com",
            Telefono = "6041234567",
            FechaRegistro = DateTime.UtcNow.AddMonths(-24),
            Activo = true
        }
    };

    context.Clientes.AddRange(clientes);
    context.SaveChanges();
    
    // Guardar referencias para crear compras
    var cliente1 = clientes[0]; // Fidelizable
    var cliente2 = clientes[1]; // No fidelizable
    var cliente3 = clientes[2]; // Fidelizable
    var cliente4 = clientes[3]; // No fidelizable
    var cliente5 = clientes[4]; // No fidelizable
    var cliente6 = clientes[5]; // No fidelizable
    var cliente7 = clientes[6]; // No fidelizable
    var cliente8 = clientes[7]; // No fidelizable
    var cliente9 = clientes[8]; // No fidelizable
    var cliente10 = clientes[9]; // Fidelizable

    // Compras variadas para todos los clientes
    var fechaUltimoMes = DateTime.UtcNow.AddMonths(-1);
    var todasLasCompras = new List<Compra>();

    // Cliente 1 - Juan Pérez (Fidelizable - más de 5M en último mes)
    todasLasCompras.AddRange(new[]
    {
        new Compra { ClienteId = cliente1.Id, NumeroFactura = "FAC-001", FechaCompra = fechaUltimoMes.AddDays(5), Monto = 2000000m, Descripcion = "Compra de productos varios", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente1.Id, NumeroFactura = "FAC-002", FechaCompra = fechaUltimoMes.AddDays(15), Monto = 3500000m, Descripcion = "Compra mayorista", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente1.Id, NumeroFactura = "FAC-003", FechaCompra = DateTime.UtcNow.AddDays(-10), Monto = 1500000m, Descripcion = "Compra reciente", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente1.Id, NumeroFactura = "FAC-008", FechaCompra = DateTime.UtcNow.AddMonths(-2), Monto = 1000000m, Descripcion = "Compra antigua", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow }
    });

    // Cliente 2 - María González (No fidelizable)
    todasLasCompras.AddRange(new[]
    {
        new Compra { ClienteId = cliente2.Id, NumeroFactura = "FAC-004", FechaCompra = fechaUltimoMes.AddDays(10), Monto = 1000000m, Descripcion = "Compra pequeña", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente2.Id, NumeroFactura = "FAC-005", FechaCompra = DateTime.UtcNow.AddDays(-5), Monto = 500000m, Descripcion = "Compra adicional", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente2.Id, NumeroFactura = "FAC-009", FechaCompra = DateTime.UtcNow.AddMonths(-2), Monto = 800000m, Descripcion = "Compra antigua", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow }
    });

    // Cliente 3 - Empresa Ejemplo (Fidelizable)
    todasLasCompras.AddRange(new[]
    {
        new Compra { ClienteId = cliente3.Id, NumeroFactura = "FAC-006", FechaCompra = fechaUltimoMes.AddDays(3), Monto = 3000000m, Descripcion = "Compra corporativa", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente3.Id, NumeroFactura = "FAC-007", FechaCompra = fechaUltimoMes.AddDays(20), Monto = 2500000m, Descripcion = "Compra adicional corporativa", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente3.Id, NumeroFactura = "FAC-010", FechaCompra = DateTime.UtcNow.AddMonths(-3), Monto = 4000000m, Descripcion = "Pedido trimestral", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow }
    });

    // Cliente 4 - Carlos Rodríguez (NO Fidelizable - menos de 5M)
    todasLasCompras.AddRange(new[]
    {
        new Compra { ClienteId = cliente4.Id, NumeroFactura = "FAC-011", FechaCompra = fechaUltimoMes.AddDays(7), Monto = 1500000m, Descripcion = "Compra de materiales", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente4.Id, NumeroFactura = "FAC-012", FechaCompra = fechaUltimoMes.AddDays(22), Monto = 1200000m, Descripcion = "Reabastecimiento", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente4.Id, NumeroFactura = "FAC-013", FechaCompra = DateTime.UtcNow.AddDays(-3), Monto = 800000m, Descripcion = "Compra urgente", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow }
    });

    // Cliente 5 - Ana Martínez (No fidelizable)
    todasLasCompras.AddRange(new[]
    {
        new Compra { ClienteId = cliente5.Id, NumeroFactura = "FAC-014", FechaCompra = fechaUltimoMes.AddDays(12), Monto = 800000m, Descripcion = "Compra personal", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente5.Id, NumeroFactura = "FAC-015", FechaCompra = DateTime.UtcNow.AddDays(-7), Monto = 600000m, Descripcion = "Compra adicional", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow }
    });

    // Cliente 6 - Roberto Silva (NO Fidelizable - Pasaporte)
    todasLasCompras.AddRange(new[]
    {
        new Compra { ClienteId = cliente6.Id, NumeroFactura = "FAC-016", FechaCompra = fechaUltimoMes.AddDays(4), Monto = 1800000m, Descripcion = "Compra internacional", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente6.Id, NumeroFactura = "FAC-017", FechaCompra = fechaUltimoMes.AddDays(18), Monto = 1200000m, Descripcion = "Importación", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente6.Id, NumeroFactura = "FAC-018", FechaCompra = DateTime.UtcNow.AddDays(-12), Monto = 900000m, Descripcion = "Pedido especial", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow }
    });

    // Cliente 7 - Distribuidora Norte (NO Fidelizable - NIT)
    todasLasCompras.AddRange(new[]
    {
        new Compra { ClienteId = cliente7.Id, NumeroFactura = "FAC-019", FechaCompra = fechaUltimoMes.AddDays(2), Monto = 2000000m, Descripcion = "Pedido mayorista", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente7.Id, NumeroFactura = "FAC-020", FechaCompra = fechaUltimoMes.AddDays(25), Monto = 1500000m, Descripcion = "Reabastecimiento mensual", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente7.Id, NumeroFactura = "FAC-021", FechaCompra = DateTime.UtcNow.AddDays(-8), Monto = 1000000m, Descripcion = "Compra adicional", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow }
    });

    // Cliente 8 - Laura Fernández (No fidelizable)
    todasLasCompras.AddRange(new[]
    {
        new Compra { ClienteId = cliente8.Id, NumeroFactura = "FAC-022", FechaCompra = fechaUltimoMes.AddDays(14), Monto = 1200000m, Descripcion = "Compra de inicio", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente8.Id, NumeroFactura = "FAC-023", FechaCompra = DateTime.UtcNow.AddDays(-2), Monto = 900000m, Descripcion = "Segunda compra", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow }
    });

    // Cliente 9 - Diego López (NO Fidelizable)
    todasLasCompras.AddRange(new[]
    {
        new Compra { ClienteId = cliente9.Id, NumeroFactura = "FAC-024", FechaCompra = fechaUltimoMes.AddDays(6), Monto = 1400000m, Descripcion = "Compra de equipos", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente9.Id, NumeroFactura = "FAC-025", FechaCompra = fechaUltimoMes.AddDays(19), Monto = 1200000m, Descripcion = "Ampliación de inventario", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente9.Id, NumeroFactura = "FAC-026", FechaCompra = DateTime.UtcNow.AddDays(-15), Monto = 800000m, Descripcion = "Compra reciente", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow }
    });

    // Cliente 10 - Sofia Ramírez (No fidelizable - Pasaporte)
    todasLasCompras.AddRange(new[]
    {
        new Compra { ClienteId = cliente10.Id, NumeroFactura = "FAC-027", FechaCompra = fechaUltimoMes.AddDays(11), Monto = 1500000m, Descripcion = "Compra única", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente10.Id, NumeroFactura = "FAC-028", FechaCompra = DateTime.UtcNow.AddDays(-6), Monto = 800000m, Descripcion = "Compra adicional", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente10.Id, NumeroFactura = "FAC-029", FechaCompra = DateTime.UtcNow.AddMonths(-2), Monto = 1200000m, Descripcion = "Compra anterior", EstadoCompraId = estadoPendiente.Id, FechaCreacion = DateTime.UtcNow }
    });

    // Cliente 10 - Comercializadora Sur (Fidelizable - NIT) - TERCER FIDELIZABLE
    todasLasCompras.AddRange(new[]
    {
        new Compra { ClienteId = cliente10.Id, NumeroFactura = "FAC-030", FechaCompra = fechaUltimoMes.AddDays(1), Monto = 6000000m, Descripcion = "Pedido corporativo grande", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente10.Id, NumeroFactura = "FAC-031", FechaCompra = fechaUltimoMes.AddDays(16), Monto = 4500000m, Descripcion = "Reabastecimiento corporativo", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow },
        new Compra { ClienteId = cliente10.Id, NumeroFactura = "FAC-032", FechaCompra = DateTime.UtcNow.AddDays(-4), Monto = 3800000m, Descripcion = "Compra adicional", EstadoCompraId = estadoCompletada.Id, FechaCreacion = DateTime.UtcNow }
    });

    context.Compras.AddRange(todasLasCompras);
    context.SaveChanges();
}
