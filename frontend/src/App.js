import React, { useState } from 'react';
import BusquedaCliente from './components/BusquedaCliente';
import InformacionCliente from './components/InformacionCliente';
import ReporteFidelizacion from './components/ReporteFidelizacion';
import Alerta from './components/Alerta';

function App() {
  const [cliente, setCliente] = useState(null);
  const [alerta, setAlerta] = useState({ mensaje: '', tipo: 'error' });

  const handleClienteEncontrado = (clienteData) => {
    setCliente(clienteData);
    setAlerta({ mensaje: 'Cliente encontrado exitosamente', tipo: 'success' });
  };

  const handleError = (mensaje) => {
    setAlerta({ mensaje, tipo: 'error' });
  };

  const cerrarAlerta = () => {
    setAlerta({ mensaje: '', tipo: 'error' });
  };

  return (
    <div className="min-h-screen bg-gray-100">
      <header className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <h1 className="text-3xl font-bold text-gray-900">
            Sistema de Consulta de Clientes
          </h1>
          <p className="text-gray-600 mt-1">Rios del Desierto SAS - Equipo SAC</p>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {alerta.mensaje && (
          <Alerta
            mensaje={alerta.mensaje}
            tipo={alerta.tipo}
            onCerrar={cerrarAlerta}
          />
        )}

        <BusquedaCliente
          onClienteEncontrado={handleClienteEncontrado}
          onError={handleError}
        />

        {cliente && (
          <InformacionCliente cliente={cliente} onError={handleError} />
        )}

        <ReporteFidelizacion onError={handleError} />
      </main>

      <footer className="bg-white border-t mt-12">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <p className="text-center text-gray-600 text-sm">
            © 2024 Rios del Desierto SAS. Todos los derechos reservados.
          </p>
        </div>
      </footer>
    </div>
  );
}

export default App;

