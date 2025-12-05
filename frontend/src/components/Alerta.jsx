import React, { useEffect } from 'react';

const Alerta = ({ mensaje, tipo = 'error', onCerrar }) => {
  useEffect(() => {
    if (mensaje) {
      const timer = setTimeout(() => {
        onCerrar();
      }, 5000);
      return () => clearTimeout(timer);
    }
  }, [mensaje, onCerrar]);

  if (!mensaje) return null;

  const estilos = {
    error: 'bg-red-50 border-red-400 text-red-800',
    success: 'bg-green-50 border-green-400 text-green-800',
    info: 'bg-blue-50 border-blue-400 text-blue-800',
  };

  return (
    <div
      className={`border-l-4 p-4 mb-4 rounded-md ${estilos[tipo]} transition-all duration-300 animate-fade-in`}
      role="alert"
    >
      <div className="flex justify-between items-center">
        <p className="font-medium">{mensaje}</p>
        <button
          onClick={onCerrar}
          className="ml-4 text-lg font-bold hover:opacity-70 transition-opacity duration-200"
        >
          ×
        </button>
      </div>
    </div>
  );
};

export default Alerta;

