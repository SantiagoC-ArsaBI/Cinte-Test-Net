# Backend - Sistema de Consulta de Clientes

## Descripción

Backend desarrollado en .NET 9 para el sistema de consulta de clientes de Rios del Desierto SAS. Implementa una API REST que permite consultar información de clientes, exportar datos y generar reportes de fidelización.

## Arquitectura

El proyecto sigue los principios de **Clean Architecture** y está dividido en las siguientes capas:

- **CinteTestNet.Domain**: Contiene las entidades del dominio (Cliente, Compra, TipoDocumento, EstadoCompra)
- **CinteTestNet.Application**: Contiene la lógica de negocio, DTOs, interfaces y servicios
- **CinteTestNet.Infrastructure**: Contiene la implementación de acceso a datos (Entity Framework Core, Repositorios)
- **CinteTestNet.API**: Capa de presentación con controladores REST

## Tecnologías Utilizadas

- .NET 9.0
- Entity Framework Core 9.0
- SQLite (Base de datos)
- ClosedXML (Generación de archivos Excel)
- Swashbuckle.AspNetCore (Swagger/OpenAPI)

## Requisitos Previos

- .NET SDK 9.0 o superior
- Visual Studio Code, Visual Studio o Rider (opcional)

## Estructura del Proyecto

```
backend/
├── CinteTestNet.API/              # Capa de presentación
│   ├── Controllers/                # Controladores REST
│   └── Program.cs                 # Configuración de la aplicación
├── CinteTestNet.Application/      # Capa de aplicación
│   ├── DTOs/                      # Data Transfer Objects
│   ├── Interfaces/                # Interfaces de repositorios y servicios
│   └── Services/                  # Servicios de negocio
├── CinteTestNet.Domain/           # Capa de dominio
│   └── Entities/                  # Entidades del dominio
└── CinteTestNet.Infrastructure/   # Capa de infraestructura
    ├── Data/                      # DbContext y configuración de EF
    └── Repositories/              # Implementación de repositorios
```

## Configuración

### Base de Datos

La aplicación utiliza SQLite como base de datos. El archivo de base de datos se crea automáticamente en la raíz del proyecto con el nombre `cinte_test.db`.

La cadena de conexión se configura en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=cinte_test.db"
  }
}
```

### CORS

La API está configurada para aceptar peticiones desde el frontend en los puertos 3000 y 3001. La configuración se encuentra en `Program.cs`.

## Endpoints de la API

### Tipos de Documento

- `GET /api/tipos-documento/` - Obtiene todos los tipos de documento activos

### Clientes

- `GET /api/clientes/buscar?tipo_documento_id={id}&numero_documento={numero}` - Busca un cliente por tipo y número de documento
- `GET /api/clientes/{id}/exportar?formato={csv|excel|txt}` - Exporta la información del cliente en el formato especificado

### Reporte de Fidelización

- `GET /api/reporte-fidelizacion/generar/` - Genera un reporte Excel con clientes elegibles para fidelización (compras > $5,000,000 COP en el último mes)

## Datos de Prueba

La aplicación incluye datos de prueba que se crean automáticamente al iniciar por primera vez:

- **Tipos de Documento**: NIT, Cédula, Pasaporte
- **Estados de Compra**: completada, pendiente, cancelada
- **Clientes de Prueba**:
  - Cliente 1: Cédula 1234567890 (Juan Pérez) - Fidelizable
  - Cliente 2: Cédula 9876543210 (María González) - No fidelizable
  - Cliente 3: NIT 900123456-1 (Empresa Ejemplo S.A.S.) - Fidelizable

## Ejecución

### Desarrollo

```bash
cd backend
dotnet run --project CinteTestNet.API
```

La API estará disponible en:
- HTTP: `http://localhost:8000`
- Swagger UI: `http://localhost:8000/swagger`

### Producción

```bash
cd backend
dotnet publish -c Release -o ./publish
cd publish
dotnet CinteTestNet.API.dll
```

## Migraciones de Base de Datos

La aplicación utiliza `EnsureCreated()` para crear la base de datos automáticamente. Para producción, se recomienda usar migraciones:

```bash
# Crear migración
dotnet ef migrations add InitialCreate --project CinteTestNet.Infrastructure --startup-project CinteTestNet.API

# Aplicar migraciones
dotnet ef database update --project CinteTestNet.Infrastructure --startup-project CinteTestNet.API
```

## Documentación de la API

La documentación interactiva de la API está disponible a través de Swagger UI cuando la aplicación está en modo desarrollo:

```
http://localhost:8000/swagger
```

## Notas de Implementación

- Se utiliza el patrón Repository para el acceso a datos
- Se implementa Unit of Work para gestionar transacciones
- Los servicios de aplicación contienen la lógica de negocio
- Los DTOs se utilizan para transferir datos entre capas
- La exportación de datos soporta formatos CSV, Excel y TXT
- El reporte de fidelización filtra clientes con compras superiores a $5,000,000 COP en el último mes

