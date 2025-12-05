# Frontend - Sistema de Consulta de Clientes

Aplicación web desarrollada con React y Tailwind CSS.

## Estructura

```
frontend/
├── public/
│   └── index.html       # HTML principal
├── src/
│   ├── App.js           # Componente principal
│   ├── index.js         # Punto de entrada
│   ├── index.css        # Estilos globales
│   ├── components/      # Componentes React
│   │   ├── BusquedaCliente.jsx
│   │   ├── InformacionCliente.jsx
│   │   ├── ReporteFidelizacion.jsx
│   │   └── Alerta.jsx
│   └── services/
│       └── api.js       # Servicios de API
├── package.json         # Dependencias
└── tailwind.config.js   # Configuración de Tailwind
```

## Componentes

### BusquedaCliente
Formulario para buscar clientes por tipo y número de documento.

### InformacionCliente
Muestra la información completa del cliente y permite exportar en diferentes formatos.

### ReporteFidelizacion
Genera y descarga el reporte de clientes elegibles para fidelización.

### Alerta
Componente para mostrar mensajes de error y éxito.

## Scripts Disponibles

```bash
# Desarrollo
npm start

# Compilar para producción
npm run build

# Ejecutar tests
npm test
```

## Variables de Entorno

Crear archivo `.env` en la raíz de `frontend/`:

```env
REACT_APP_API_URL=http://localhost:5173/api
```

## Tecnologías

- React 18.2.0
- Tailwind CSS 3.3.6
- Axios 1.6.2
- File Saver 2.0.5

## Estilos

La aplicación usa Tailwind CSS para todos los estilos. Las animaciones son suaves y sutiles según las preferencias del proyecto.

