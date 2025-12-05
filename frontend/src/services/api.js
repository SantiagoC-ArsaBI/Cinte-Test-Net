import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:8000/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const tiposDocumentoAPI = {
  getAll: () => api.get('/tipos-documento/'),
};

export const clientesAPI = {
  buscar: (tipoDocumentoId, numeroDocumento) =>
    api.get('/clientes/buscar/', {
      params: {
        tipo_documento_id: tipoDocumentoId,
        numero_documento: numeroDocumento,
      },
    }),
  exportar: (clienteId, formato) =>
    api.get(`/clientes/${clienteId}/exportar/`, {
      params: { formato },
      responseType: 'blob',
    }),
};

export const reporteAPI = {
  generarFidelizacion: () =>
    api.get('/reporte-fidelizacion/generar/', {
      responseType: 'blob',
    }),
};

export default api;

