# Guía de Implementación - Sistema de Consulta de Clientes

## Requisitos Previos

Antes de comenzar la implementación, asegúrese de tener instalado:

1. **.NET SDK 9.0** o superior
   - Verificar instalación: `dotnet --version`
   - Descarga: https://dotnet.microsoft.com/download

2. **Git** (opcional, para clonar el repositorio)

## Paso 1: Preparación del Entorno

### 1.1. Clonar o Descargar el Proyecto

Si el proyecto está en un repositorio Git:

```bash
git clone <url-del-repositorio>
cd Cinte-Test-Net/backend
```

Si tiene el proyecto localmente, navegue al directorio:

```bash
cd Cinte-Test-Net/backend
```

### 1.2. Restaurar Dependencias

```bash
dotnet restore
```

Este comando descargará todos los paquetes NuGet necesarios.

## Paso 2: Configuración de la Base de Datos

### 2.1. Verificar Cadena de Conexión

La cadena de conexión está configurada en `CinteTestNet.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=cinte_test.db"
  }
}
```

Para producción, puede cambiar la ruta de la base de datos:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/ruta/produccion/cinte_test.db"
  }
}
```

### 2.2. Crear la Base de Datos

La base de datos se crea automáticamente la primera vez que se ejecuta la aplicación. No se requieren pasos adicionales.

## Paso 3: Configuración de CORS

Si necesita permitir peticiones desde otros orígenes, edite `CinteTestNet.API/Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://dominio.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

## Paso 4: Ejecución en Desarrollo

### 4.1. Ejecutar la Aplicación

```bash
dotnet run --project CinteTestNet.API
```

O desde el directorio del proyecto API:

```bash
cd CinteTestNet.API
dotnet run
```

### 4.2. Verificar que la API Está Funcionando

1. Abra un navegador y vaya a: `http://localhost:8000/swagger`
2. Debería ver la documentación interactiva de Swagger
3. Pruebe el endpoint `GET /api/tipos-documento/` para verificar que funciona

## Paso 5: Despliegue en Producción

### 5.1. Compilar para Producción

```bash
dotnet publish -c Release -o ./publish
```

Esto creará una carpeta `publish` con todos los archivos necesarios.

### 5.2. Configurar Variables de Entorno

Cree un archivo `appsettings.Production.json` o configure variables de entorno:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=/ruta/produccion/cinte_test.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

### 5.3. Ejecutar en Producción

**Opción 1: Ejecutar directamente**

```bash
cd publish
dotnet CinteTestNet.API.dll
```

**Opción 2: Usar un servidor web (recomendado)**

Para producción, se recomienda usar un servidor web como:
- **IIS** (Windows)
- **Nginx** + **systemd** (Linux)
- **Docker** (cualquier plataforma)

### 5.4. Ejemplo con systemd (Linux)

Cree un archivo de servicio `/etc/systemd/system/cinte-api.service`:

```ini
[Unit]
Description=Cinte Test Net API
After=network.target

[Service]
Type=notify
WorkingDirectory=/ruta/a/publish
ExecStart=/usr/bin/dotnet /ruta/a/publish/CinteTestNet.API.dll
Restart=always
RestartSec=10
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

Habilitar y iniciar el servicio:

```bash
sudo systemctl enable cinte-api
sudo systemctl start cinte-api
sudo systemctl status cinte-api
```

### 5.5. Ejemplo con Docker

Cree un `Dockerfile` en la raíz del proyecto:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CinteTestNet.API/CinteTestNet.API.csproj", "CinteTestNet.API/"]
COPY ["CinteTestNet.Application/CinteTestNet.Application.csproj", "CinteTestNet.Application/"]
COPY ["CinteTestNet.Domain/CinteTestNet.Domain.csproj", "CinteTestNet.Domain/"]
COPY ["CinteTestNet.Infrastructure/CinteTestNet.Infrastructure.csproj", "CinteTestNet.Infrastructure/"]
RUN dotnet restore "CinteTestNet.API/CinteTestNet.API.csproj"
COPY . .
WORKDIR "/src/CinteTestNet.API"
RUN dotnet build "CinteTestNet.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CinteTestNet.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CinteTestNet.API.dll"]
```

Construir y ejecutar:

```bash
docker build -t cinte-api .
docker run -d -p 8000:80 --name cinte-api cinte-api
```

## Paso 6: Configuración del Frontend

Asegúrese de que el frontend esté configurado para apuntar a la URL correcta de la API.

En el archivo `.env` del frontend o en `src/services/api.js`:

```javascript
const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:8000/api';
```

Para producción, configure la variable de entorno:

```bash
REACT_APP_API_URL=http://servidor:8000/api
```

## Paso 7: Verificación Post-Implementación

### 7.1. Verificar Endpoints

1. **Tipos de Documento**: `GET http://localhost:8000/api/tipos-documento/`
2. **Buscar Cliente**: `GET http://localhost:8000/api/clientes/buscar?tipo_documento_id=2&numero_documento=1234567890`
3. **Exportar Cliente**: `GET http://localhost:8000/api/clientes/1/exportar?formato=excel`
4. **Reporte Fidelización**: `GET http://localhost:8000/api/reporte-fidelizacion/generar/`

### 7.2. Verificar Base de Datos

La base de datos SQLite se crea automáticamente. Puede verificar su existencia:

```bash
ls -la cinte_test.db
```

Para inspeccionar la base de datos, puede usar herramientas como:
- **DB Browser for SQLite** (GUI)
- **sqlite3** (línea de comandos)

```bash
sqlite3 cinte_test.db
.tables
SELECT * FROM Clientes;
```

## Paso 8: Mantenimiento

### 8.1. Backup de Base de Datos

Realice backups periódicos de la base de datos SQLite:

```bash
cp cinte_test.db cinte_test_backup_$(date +%Y%m%d).db
```

### 8.2. Logs

Los logs se configuran en `appsettings.json`. Para producción, considere usar un sistema de logging más robusto como Serilog.

### 8.3. Actualizaciones

Para actualizar el proyecto:

```bash
git pull
dotnet restore
dotnet build
dotnet publish -c Release -o ./publish
```

## Solución de Problemas

### Error: "No se puede conectar a la base de datos"

- Verifique que la ruta de la base de datos sea correcta
- Verifique los permisos de escritura en el directorio

### Error: "CORS policy"

- Verifique la configuración de CORS en `Program.cs`
- Asegúrese de que el origen del frontend esté incluido

### Error: "Puerto en uso"

- Cambie el puerto en `launchSettings.json` o `appsettings.json`
- O detenga el proceso que está usando el puerto

### La base de datos no se crea

- Verifique los permisos del directorio
- Revise los logs de la aplicación para errores específicos

## Contacto y Soporte

Para problemas o preguntas sobre la implementación, consulte la documentación técnica en `README.md` o contacte al equipo de desarrollo.

