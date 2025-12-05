import React from 'react';
import { clientesAPI } from '../services/api';
import { saveAs } from 'file-saver';

const InformacionCliente = ({ cliente, onError }) => {
  const formatearFecha = (fecha) => {
    if (!fecha) return '-';
    const date = new Date(fecha);
    return date.toLocaleDateString('es-CO', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const formatearFechaCorta = (fecha) => {
    if (!fecha) return '-';
    const date = new Date(fecha);
    return date.toLocaleDateString('es-CO', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
    });
  };

  const formatearMoneda = (monto) => {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0,
    }).format(monto);
  };

  const handleExportar = async (formato) => {
    try {
      const response = await clientesAPI.exportar(cliente.id, formato);
      const blob = new Blob([response.data]);
      const extension = formato === 'excel' ? 'xlsx' : formato;
      const filename = `cliente_${cliente.numero_documento}.${extension}`;
      saveAs(blob, filename);
    } catch (error) {
      console.error('Error al exportar:', error);
      onError('Error al exportar la información del cliente');
    }
  };

  if (!cliente) {
    return null;
  }

  return (
    <div className="bg-white rounded-lg shadow-md p-6 mb-6">
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-2xl font-bold text-gray-800">Información del Cliente</h2>
        <div className="flex gap-2">
          <button
            onClick={() => handleExportar('csv')}
            className="px-4 py-2 bg-green-600 text-white text-sm font-medium rounded-md hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 focus:ring-offset-2 transition-all duration-200 transform hover:scale-105 active:scale-95"
          >
            Exportar CSV
          </button>
          <button
            onClick={() => handleExportar('excel')}
            className="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all duration-200 transform hover:scale-105 active:scale-95"
          >
            Exportar Excel
          </button>
          <button
            onClick={() => handleExportar('txt')}
            className="px-4 py-2 bg-gray-600 text-white text-sm font-medium rounded-md hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2 transition-all duration-200 transform hover:scale-105 active:scale-95"
          >
            Exportar TXT
          </button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
        <div className="bg-gray-50 p-4 rounded-md">
          <p className="text-sm text-gray-600">Tipo de Documento</p>
          <p className="text-lg font-semibold text-gray-800">
            {cliente.tipo_documento?.nombre || '-'}
          </p>
        </div>

        <div className="bg-gray-50 p-4 rounded-md">
          <p className="text-sm text-gray-600">Número de Documento</p>
          <p className="text-lg font-semibold text-gray-800">{cliente.numero_documento}</p>
        </div>

        <div className="bg-gray-50 p-4 rounded-md">
          <p className="text-sm text-gray-600">Nombre</p>
          <p className="text-lg font-semibold text-gray-800">{cliente.nombre}</p>
        </div>

        <div className="bg-gray-50 p-4 rounded-md">
          <p className="text-sm text-gray-600">Apellido</p>
          <p className="text-lg font-semibold text-gray-800">{cliente.apellido}</p>
        </div>

        <div className="bg-gray-50 p-4 rounded-md">
          <p className="text-sm text-gray-600">Correo Electrónico</p>
          <p className="text-lg font-semibold text-gray-800">{cliente.correo}</p>
        </div>

        <div className="bg-gray-50 p-4 rounded-md">
          <p className="text-sm text-gray-600">Teléfono</p>
          <p className="text-lg font-semibold text-gray-800">{cliente.telefono}</p>
        </div>

        <div className="bg-gray-50 p-4 rounded-md">
          <p className="text-sm text-gray-600">Fecha de Registro</p>
          <p className="text-lg font-semibold text-gray-800">
            {formatearFecha(cliente.fecha_registro)}
          </p>
        </div>

        <div className="bg-gray-50 p-4 rounded-md">
          <p className="text-sm text-gray-600">Total de Compras</p>
          <p className="text-lg font-semibold text-gray-800">
            {cliente.total_compras || cliente.compras?.length || 0}
          </p>
        </div>
      </div>

      {cliente.compras && cliente.compras.length > 0 && (
        <div className="mt-6">
          <div className="flex justify-between items-center mb-4">
            <h3 className="text-xl font-bold text-gray-800">Historial de Compras</h3>
            <div className="flex gap-4 text-sm">
              <div className="bg-blue-50 px-4 py-2 rounded-md">
                <span className="text-gray-600">Total Compras: </span>
                <span className="font-semibold text-gray-800">
                  {cliente.compras.filter(c => c.estado === 'completada').length}
                </span>
              </div>
              {cliente.monto_total_compras && (
                <div className="bg-green-50 px-4 py-2 rounded-md">
                  <span className="text-gray-600">Monto Total: </span>
                  <span className="font-semibold text-gray-800">
                    {formatearMoneda(cliente.monto_total_compras)}
                  </span>
                </div>
              )}
            </div>
          </div>
          <div className="overflow-x-auto shadow-sm rounded-lg border border-gray-200">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Número Factura
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Fecha
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Descripción
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Monto
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Estado
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {cliente.compras.map((compra) => (
                  <tr key={compra.id} className="hover:bg-gray-50 transition-colors duration-150">
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                      {compra.numero_factura}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {formatearFechaCorta(compra.fecha_compra)}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600 max-w-xs">
                      {compra.descripcion || '-'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold text-gray-900">
                      {formatearMoneda(compra.monto)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span
                        className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                          compra.estado === 'completada'
                            ? 'bg-green-100 text-green-800'
                            : compra.estado === 'pendiente'
                            ? 'bg-yellow-100 text-yellow-800'
                            : 'bg-red-100 text-red-800'
                        }`}
                      >
                        {compra.estado}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
      
      {(!cliente.compras || cliente.compras.length === 0) && (
        <div className="mt-6 p-4 bg-gray-50 rounded-md text-center">
          <p className="text-gray-600">Este cliente no tiene compras registradas.</p>
        </div>
      )}
    </div>
  );
};

export default InformacionCliente;

