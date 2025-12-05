import React, { useState, useEffect } from 'react';
import { tiposDocumentoAPI, clientesAPI } from '../services/api';

const BusquedaCliente = ({ onClienteEncontrado, onError }) => {
  const [tiposDocumento, setTiposDocumento] = useState([]);
  const [tipoDocumentoId, setTipoDocumentoId] = useState('');
  const [numeroDocumento, setNumeroDocumento] = useState('');
  const [cargando, setCargando] = useState(false);
  const [cargandoTipos, setCargandoTipos] = useState(true);

  useEffect(() => {
    cargarTiposDocumento();
  }, []);

  const cargarTiposDocumento = async () => {
    try {
      setCargandoTipos(true);
      const response = await tiposDocumentoAPI.getAll();
      // Manejar respuesta paginada o directa
      const tipos = response.data.results || response.data || [];
      setTiposDocumento(tipos);
      if (tipos && tipos.length > 0) {
        setTipoDocumentoId(tipos[0].id.toString());
      } else {
        onError('No se pudieron cargar los tipos de documento');
      }
    } catch (error) {
      onError('Error al cargar tipos de documento. Verifique que el backend esté corriendo en http://localhost:8000');
    } finally {
      setCargandoTipos(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    // Validación mejorada
    const tipoDocId = tipoDocumentoId?.toString().trim();
    const numDoc = numeroDocumento?.toString().trim();
    
    if (!tipoDocId || tipoDocId === '' || !numDoc || numDoc === '') {
      onError('Por favor complete todos los campos');
      return;
    }

    try {
      setCargando(true);
      const response = await clientesAPI.buscar(tipoDocId, numDoc);
      onClienteEncontrado(response.data);
    } catch (error) {
      if (error.response) {
        if (error.response.status === 404) {
          onError('Cliente no encontrado');
        } else if (error.response.status === 400) {
          onError(error.response.data?.error || 'Datos inválidos');
        } else {
          onError('Error al buscar el cliente. Por favor intente nuevamente.');
        }
      } else if (error.request) {
        onError('No se pudo conectar con el servidor. Verifique que el backend esté corriendo.');
      } else {
        onError('Error al buscar el cliente. Por favor intente nuevamente.');
      }
    } finally {
      setCargando(false);
    }
  };

  return (
    <div className="bg-white rounded-lg shadow-md p-6 mb-6">
      <h2 className="text-2xl font-bold text-gray-800 mb-4">Buscar Cliente</h2>
      <form onSubmit={handleSubmit} className="space-y-4">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div>
            <label htmlFor="tipoDocumento" className="block text-sm font-medium text-gray-700 mb-2">
              Tipo de Documento
            </label>
            <select
              id="tipoDocumento"
              value={tipoDocumentoId || ''}
              onChange={(e) => setTipoDocumentoId(e.target.value)}
              disabled={cargandoTipos || cargando}
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary-500 focus:border-primary-500 disabled:bg-gray-100 disabled:cursor-not-allowed transition-all duration-200"
            >
              {cargandoTipos ? (
                <option value="">Cargando...</option>
              ) : tiposDocumento.length === 0 ? (
                <option value="">No hay tipos disponibles</option>
              ) : (
                <>
                  <option value="">Seleccione un tipo</option>
                  {tiposDocumento.map((tipo) => (
                    <option key={tipo.id} value={tipo.id.toString()}>
                      {tipo.nombre}
                    </option>
                  ))}
                </>
              )}
            </select>
          </div>

          <div>
            <label htmlFor="numeroDocumento" className="block text-sm font-medium text-gray-700 mb-2">
              Número de Documento
            </label>
            <input
              type="text"
              id="numeroDocumento"
              value={numeroDocumento}
              onChange={(e) => setNumeroDocumento(e.target.value)}
              disabled={cargando}
              placeholder="Ingrese el número de documento"
              className="w-full px-4 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-primary-500 focus:border-primary-500 disabled:bg-gray-100 disabled:cursor-not-allowed transition-all duration-200"
            />
          </div>

          <div className="flex items-end">
            <button
              type="submit"
              disabled={cargando || cargandoTipos}
              className="w-full px-6 py-2 bg-primary-600 text-white font-medium rounded-md hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2 disabled:bg-gray-400 disabled:cursor-not-allowed transition-all duration-200 transform hover:scale-105 active:scale-95"
            >
              {cargando ? 'Buscando...' : 'Buscar'}
            </button>
          </div>
        </div>
      </form>
    </div>
  );
};

export default BusquedaCliente;

