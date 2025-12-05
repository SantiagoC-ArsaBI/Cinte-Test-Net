# Cinte-Test-Net

Sistema de Consulta de Clientes desarrollado para Rios del Desierto SAS - Equipo SAC.

## 📋 Descripción

Sistema web completo que permite consultar información de clientes, exportar datos en múltiples formatos y generar reportes de fidelización. El proyecto está compuesto por un frontend desarrollado en React y un backend desarrollado en .NET 9.

## 🏗️ Arquitectura del Proyecto

```
Cinte-Test-Net/
├── backend/              # API REST desarrollada en .NET 9
│   ├── CinteTestNet.API/           # Capa de presentación (Controladores)
│   ├── CinteTestNet.Application/   # Capa de aplicación (Servicios, DTOs)
│   ├── CinteTestNet.Domain/        # Capa de dominio (Entidades)
│   └── CinteTestNet.Infrastructure/# Capa de infraestructura (EF Core, Repositorios)
├── frontend/             # Aplicación web React
└── Docs/                 # Documentación del proyecto
```

## 🚀 Inicio Rápido

### Prerrequisitos

- **.NET SDK 9.0** o superior
- **Node.js** 18.x o superior
- **npm** o **yarn**

### Instalación y Ejecución

#### Backend

```bash
# Navegar al directorio del backend
cd backend

# Restaurar dependencias
dotnet restore

# Ejecutar la aplicación
dotnet run --project CinteTestNet.API
```

El backend estará disponible en:
- **API**: `http://localhost:8000`
- **Swagger UI**: `http://localhost:8000/swagger`

#### Frontend

```bash
# Navegar al directorio del frontend
cd frontend

# Instalar dependencias
npm install

# Ejecutar en modo desarrollo
npm start
```

El frontend estará disponible en `http://localhost:3000`

## 📚 Funcionalidades

### Consulta de Clientes
- Búsqueda de clientes por tipo y número de documento
- Visualización completa de información del cliente
- Historial de compras con detalles

### Exportación de Datos
- **CSV**: Formato delimitado por comas
- **Excel**: Archivo XLSX con formato
- **TXT**: Archivo de texto plano formateado

### Reporte de Fidelización
- Generación automática de reporte Excel
- Filtrado de clientes con compras superiores a $5,000,000 COP en el último mes
- Incluye información completa de clientes elegibles

## 🛠️ Tecnologías Utilizadas

### Backend
- **.NET 9.0**: Framework principal
- **Entity Framework Core 9.0**: ORM para acceso a datos
- **SQLite**: Base de datos
- **ClosedXML**: Generación de archivos Excel
- **Swashbuckle.AspNetCore**: Documentación Swagger/OpenAPI

### Frontend
- **React 18.2.0**: Biblioteca de UI
- **Tailwind CSS 3.3.6**: Framework de estilos
- **Axios 1.6.2**: Cliente HTTP
- **File Saver 2.0.5**: Descarga de archivos
- **XLSX 0.18.5**: Manipulación de archivos Excel

## 📁 Estructura del Proyecto

### Backend (Clean Architecture)

```
backend/
├── CinteTestNet.API/              # Capa de presentación
│   ├── Controllers/                # Controladores REST
│   ├── Program.cs                  # Configuración de la aplicación
│   └── appsettings.json            # Configuración
├── CinteTestNet.Application/      # Capa de aplicación
│   ├── DTOs/                       # Data Transfer Objects
│   ├── Interfaces/                 # Interfaces de repositorios
│   └── Services/                   # Servicios de negocio
├── CinteTestNet.Domain/           # Capa de dominio
│   └── Entities/                   # Entidades del dominio
└── CinteTestNet.Infrastructure/   # Capa de infraestructura
    ├── Data/                       # DbContext
    └── Repositories/               # Implementación de repositorios
```

### Frontend

```
frontend/
├── src/
│   ├── components/                 # Componentes React
│   │   ├── BusquedaCliente.jsx
│   │   ├── InformacionCliente.jsx
│   │   ├── ReporteFidelizacion.jsx
│   │   └── Alerta.jsx
│   ├── services/                   # Servicios de API
│   │   └── api.js
│   ├── App.js                      # Componente principal
│   └── index.js                    # Punto de entrada
└── package.json                    # Dependencias
```

## 🔌 Endpoints de la API

### Tipos de Documento
- `GET /api/tipos-documento/` - Obtiene todos los tipos de documento

### Clientes
- `GET /api/clientes/buscar?tipo_documento_id={id}&numero_documento={numero}` - Busca un cliente
- `GET /api/clientes/{id}/exportar?formato={csv|excel|txt}` - Exporta información del cliente

### Reporte de Fidelización
- `GET /api/reporte-fidelizacion/generar/` - Genera reporte Excel de clientes fidelizables

## 🗄️ Base de Datos

El proyecto utiliza **SQLite** como base de datos. El archivo se crea automáticamente en:
```
backend/CinteTestNet.API/cinte_test.db
```

### Modelo de Datos

- **TiposDocumento**: Tipos de documento de identidad (NIT, Cédula, Pasaporte)
- **Clientes**: Información de clientes
- **Compras**: Historial de compras de clientes
- **EstadosCompra**: Estados de las compras (completada, pendiente, cancelada)

### Datos de Prueba

El sistema incluye datos de prueba que se crean automáticamente:
- 3 tipos de documento
- 3 estados de compra
- 10 clientes de ejemplo
- Múltiples compras distribuidas entre clientes

## 📖 Documentación

- **Backend**: Ver `backend/README.md` para documentación técnica completa
- **Guía de Implementación**: Ver `backend/GUIA_IMPLEMENTACION.md`
- **Documentación Técnica**: Ver `backend/DOCUMENTACION_TECNICA.md`
- **Frontend**: Ver `frontend/README.md`

## 🔧 Configuración

### Variables de Entorno

#### Frontend
Crear archivo `.env` en `frontend/`:
```env
REACT_APP_API_URL=http://localhost:8000/api
```

#### Backend
La configuración se encuentra en `backend/CinteTestNet.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=cinte_test.db"
  }
}
```

## 🧪 Pruebas

### Clientes de Prueba Disponibles

**Clientes Fidelizables** (compras > $5,000,000 en último mes):
- Juan Pérez - Cédula: `1234567890`
- Empresa Ejemplo S.A.S. - NIT: `900123456-1`
- Comercializadora Sur Ltda. - NIT: `800111222-3`

**Otros Clientes**:
- María González - Cédula: `9876543210`
- Carlos Rodríguez - Cédula: `1122334455`
- Ana Martínez - Cédula: `5566778899`
- Roberto Silva - Pasaporte: `AB123456`
- Distribuidora Norte S.A. - NIT: `900987654-2`
- Laura Fernández - Cédula: `2233445566`
- Diego López - Cédula: `7788990011`

## 🚀 Despliegue

### Backend

```bash
# Compilar para producción
dotnet publish -c Release -o ./publish

# Ejecutar
cd publish
dotnet CinteTestNet.API.dll
```

### Frontend

```bash
# Compilar para producción
npm run build

# Los archivos estarán en frontend/build/
```

## 📝 Notas de Desarrollo

- El backend utiliza **Clean Architecture** para separación de responsabilidades
- La serialización JSON utiliza **snake_case** para compatibilidad con el frontend
- CORS está configurado para permitir peticiones desde `localhost:3000` y `localhost:3001`
- La base de datos se crea automáticamente al iniciar la aplicación

## 🤝 Contribución

Este proyecto fue desarrollado como parte de una prueba técnica para Rios del Desierto SAS.

## 📄 Licencia

Proyecto desarrollado para Rios del Desierto SAS - Equipo SAC.

---

**Desarrollado con ❤️ usando .NET 9 y React**
