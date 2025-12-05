import React, { useState } from 'react';
import { reporteAPI } from '../services/api';
import { saveAs } from 'file-saver';

const ReporteFidelizacion = ({ onError }) => {
  const [generando, setGenerando] = useState(false);

  const handleGenerarReporte = async () => {
    try {
      setGenerando(true);
      const response = await reporteAPI.generarFidelizacion();
      const blob = new Blob([response.data], {
        type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      });
      const fecha = new Date().toISOString().split('T')[0];
      const filename = `reporte_fidelizacion_${fecha}.xlsx`;
      saveAs(blob, filename);
    } catch (error) {
      console.error('Error al generar reporte:', error);
      if (error.response && error.response.status === 404) {
        onError('No hay clientes que cumplan los criterios de fidelización');
      } else {
        onError('Error al generar el reporte. Por favor intente nuevamente.');
      }
    } finally {
      setGenerando(false);
    }
  };

  return (
    <div className="bg-white rounded-lg shadow-md p-6">
      <h2 className="text-2xl font-bold text-gray-800 mb-4">Reporte de Fidelización</h2>
      <p className="text-gray-600 mb-4">
        Genera un reporte en Excel con los clientes elegibles para fidelización.
        Se incluyen clientes con compras superiores a $5'000.000 COP en el último mes.
      </p>
      <button
        onClick={handleGenerarReporte}
        disabled={generando}
        className="px-6 py-3 bg-purple-600 text-white font-medium rounded-md hover:bg-purple-700 focus:outline-none focus:ring-2 focus:ring-purple-500 focus:ring-offset-2 disabled:bg-gray-400 disabled:cursor-not-allowed transition-all duration-200 transform hover:scale-105 active:scale-95"
      >
        {generando ? 'Generando...' : 'Generar Reporte Excel'}
      </button>
    </div>
  );
};

export default ReporteFidelizacion;

