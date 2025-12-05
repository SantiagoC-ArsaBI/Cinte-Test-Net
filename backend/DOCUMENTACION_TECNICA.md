# Documentación Técnica - Sistema de Consulta de Clientes

## Arquitectura del Sistema

### Clean Architecture

El proyecto sigue los principios de Clean Architecture, separando las responsabilidades en capas independientes:

```
┌─────────────────────────────────────┐
│         CinteTestNet.API             │  ← Capa de Presentación
│      (Controladores REST)            │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      CinteTestNet.Application        │  ← Capa de Aplicación
│   (Servicios, DTOs, Interfaces)      │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│        CinteTestNet.Domain           │  ← Capa de Dominio
│        (Entidades del Negocio)      │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│     CinteTestNet.Infrastructure      │  ← Capa de Infraestructura
│   (EF Core, Repositorios, Data)     │
└─────────────────────────────────────┘
```

### Principios Aplicados

1. **Dependency Inversion**: Las capas superiores no dependen de las inferiores, sino de abstracciones (interfaces)
2. **Separation of Concerns**: Cada capa tiene una responsabilidad específica
3. **Repository Pattern**: Abstracción del acceso a datos
4. **Unit of Work**: Gestión de transacciones y consistencia de datos

## Modelo de Datos

### Diagrama de Entidades

```
┌──────────────────┐
│ TipoDocumento   │
├──────────────────┤
│ Id (PK)          │
│ Nombre           │
│ Codigo           │
│ Activo           │
└────────┬─────────┘
         │ 1
         │
         │ N
┌────────▼─────────┐
│ Cliente          │
├──────────────────┤
│ Id (PK)          │
│ TipoDocumentoId  │──┐
│ NumeroDocumento  │  │ FK
│ Nombre           │  │
│ Apellido         │  │
│ Correo           │  │
│ Telefono         │  │
│ FechaRegistro    │  │
│ Activo           │  │
└────────┬─────────┘  │
         │ 1          │
         │            │
         │ N          │
┌────────▼─────────┐  │
│ Compra           │  │
├──────────────────┤  │
│ Id (PK)          │  │
│ ClienteId        │──┘
│ NumeroFactura    │
│ FechaCompra      │
│ Monto            │
│ Descripcion      │
│ EstadoCompraId   │──┐
│ FechaCreacion    │  │
└──────────────────┘  │
                      │
         ┌────────────┘
         │
         │ N
┌────────▼─────────┐
│ EstadoCompra     │
├──────────────────┤
│ Id (PK)          │
│ Nombre           │
│ Codigo           │
│ Activo           │
└──────────────────┘
```

### Entidades del Dominio

#### TipoDocumento
- **Propósito**: Representa los tipos de documento de identidad (NIT, Cédula, Pasaporte)
- **Campos Clave**:
  - `Codigo`: Código único (NIT, CC, PA)
  - `Nombre`: Nombre descriptivo

#### Cliente
- **Propósito**: Representa a un cliente del sistema
- **Campos Clave**:
  - `TipoDocumentoId` + `NumeroDocumento`: Identificación única
  - `Activo`: Soft delete

#### Compra
- **Propósito**: Representa una transacción de compra realizada por un cliente
- **Campos Clave**:
  - `NumeroFactura`: Identificador único de la factura
  - `Monto`: Valor de la compra
  - `FechaCompra`: Fecha de la transacción
  - `EstadoCompraId`: Estado de la compra

#### EstadoCompra
- **Propósito**: Representa los estados posibles de una compra
- **Valores**: completada, pendiente, cancelada

## Patrones de Diseño Implementados

### 1. Repository Pattern

**Ubicación**: `CinteTestNet.Infrastructure/Repositories`

**Propósito**: Abstraer el acceso a datos y facilitar testing y mantenimiento.

**Interfaces**:
- `IRepository<T>`: Operaciones CRUD genéricas
- `IClienteRepository`: Operaciones específicas de clientes

**Implementación**:
- `Repository<T>`: Implementación genérica con Entity Framework Core
- `ClienteRepository`: Extiende el repositorio genérico con métodos específicos

### 2. Unit of Work Pattern

**Ubicación**: `CinteTestNet.Infrastructure/Repositories/UnitOfWork.cs`

**Propósito**: Gestionar transacciones y mantener consistencia en las operaciones de base de datos.

**Características**:
- Agrupa múltiples repositorios
- Gestiona transacciones
- Implementa `SaveChangesAsync()` para persistir cambios

### 3. DTO (Data Transfer Object)

**Ubicación**: `CinteTestNet.Application/DTOs`

**Propósito**: Transferir datos entre capas sin exponer las entidades del dominio.

**DTOs Principales**:
- `ClienteDto`: Información del cliente con compras
- `TipoDocumentoDto`: Información del tipo de documento
- `CompraDto`: Información de una compra
- `ClienteFidelizacionDto`: Datos para el reporte de fidelización

## Servicios de Aplicación

### ClienteService

**Ubicación**: `CinteTestNet.Application/Services/ClienteService.cs`

**Responsabilidades**:
1. Buscar clientes por documento
2. Obtener tipos de documento
3. Exportar información de clientes (CSV, Excel, TXT)
4. Generar reporte de fidelización

**Métodos Principales**:

```csharp
Task<ClienteDto?> BuscarClientePorDocumentoAsync(int tipoDocumentoId, string numeroDocumento)
Task<IEnumerable<TipoDocumentoDto>> ObtenerTiposDocumentoAsync()
Task<byte[]> ExportarClienteAsync(int clienteId, string formato)
Task<byte[]> GenerarReporteFidelizacionAsync()
```

## Controladores API

### TiposDocumentoController

**Ruta Base**: `/api/tipos-documento`

**Endpoints**:
- `GET /` - Lista todos los tipos de documento activos

### ClientesController

**Ruta Base**: `/api/clientes`

**Endpoints**:
- `GET /buscar?tipo_documento_id={id}&numero_documento={numero}` - Busca un cliente
- `GET /{id}/exportar?formato={csv|excel|txt}` - Exporta información del cliente

### ReporteFidelizacionController

**Ruta Base**: `/api/reporte-fidelizacion`

**Endpoints**:
- `GET /generar/` - Genera reporte Excel de clientes fidelizables

## Configuración de Entity Framework Core

### ApplicationDbContext

**Ubicación**: `CinteTestNet.Infrastructure/Data/ApplicationDbContext.cs`

**Configuraciones**:
- Relaciones entre entidades
- Índices únicos
- Restricciones de eliminación (Restrict)
- Configuración de propiedades (longitud, requeridos, etc.)

### Configuración de Relaciones

- **Cliente → TipoDocumento**: Many-to-One (Restrict on delete)
- **Cliente → Compra**: One-to-Many (Restrict on delete)
- **Compra → EstadoCompra**: Many-to-One (Restrict on delete)

## Exportación de Datos

### Formatos Soportados

1. **CSV**: Formato de texto delimitado por comas
2. **Excel**: Archivo XLSX usando ClosedXML
3. **TXT**: Archivo de texto plano formateado

### Implementación

La exportación se realiza en `ClienteService`:
- `ExportarCsv()`: Genera CSV con información del cliente y compras
- `ExportarExcel()`: Crea archivo Excel con formato y estilos
- `ExportarTxt()`: Genera texto plano formateado

## Reporte de Fidelización

### Lógica de Negocio

El reporte identifica clientes elegibles para fidelización basándose en:

1. **Período**: Último mes (30 días)
2. **Monto Mínimo**: $5,000,000 COP
3. **Estado**: Solo compras completadas

### Implementación

El método `GetClientesFidelizablesAsync()` en `ClienteRepository`:
1. Filtra clientes activos con compras en el período
2. Incluye solo compras completadas
3. Calcula el monto total por cliente
4. Filtra clientes que superen el monto mínimo

## Inyección de Dependencias

### Configuración en Program.cs

```csharp
// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(...);

// Repositorios y Servicios
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IClienteService, ClienteService>();
```

### Ciclo de Vida

- **DbContext**: Scoped (una instancia por request)
- **UnitOfWork**: Scoped
- **Servicios**: Scoped

## Seguridad

### CORS

Configurado para permitir peticiones desde:
- `http://localhost:3000`
- `http://localhost:3001`

Para producción, actualizar los orígenes permitidos.

### Validación

- Validación de parámetros en controladores
- Manejo de errores con códigos HTTP apropiados
- Validación de formatos de exportación

## Optimizaciones Implementadas

1. **Eager Loading**: Uso de `Include()` para cargar relaciones necesarias
2. **Índices**: Índices únicos en campos clave (NumeroDocumento, NumeroFactura)
3. **Filtrado en Base de Datos**: Filtros aplicados en queries, no en memoria
4. **Lazy Loading Deshabilitado**: Control explícito de qué datos se cargan

## Testing

### Estructura Recomendada

```
CinteTestNet.Tests/
├── Unit/
│   ├── Services/
│   └── Repositories/
└── Integration/
    └── API/
```

### Ejemplo de Test Unitario

```csharp
[Fact]
public async Task BuscarClientePorDocumento_ClienteExiste_RetornaCliente()
{
    // Arrange
    var cliente = new Cliente { ... };
    // Act
    var resultado = await _service.BuscarClientePorDocumentoAsync(...);
    // Assert
    Assert.NotNull(resultado);
}
```

## Mejoras Futuras

1. **Autenticación y Autorización**: Implementar JWT o Identity
2. **Paginación**: Agregar paginación a listados
3. **Caching**: Implementar cache para consultas frecuentes
4. **Logging**: Integrar Serilog o similar
5. **Migraciones**: Usar migraciones de EF Core en lugar de EnsureCreated
6. **Validación**: Agregar FluentValidation
7. **Testing**: Implementar suite completa de tests

## Consideraciones de Rendimiento

1. **Base de Datos**: SQLite es adecuado para desarrollo y pequeñas implementaciones. Para producción con alto volumen, considerar SQL Server o PostgreSQL
2. **Exportación**: Para grandes volúmenes, considerar procesamiento asíncrono
3. **Reportes**: Para reportes complejos, considerar procesamiento en background

## Referencias

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [ClosedXML](https://github.com/ClosedXML/ClosedXML)
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)

