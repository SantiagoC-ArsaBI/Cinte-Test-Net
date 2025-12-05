using CinteTestNet.Application.DTOs;
using CinteTestNet.Application.Interfaces;
using CinteTestNet.Domain.Entities;
using ClosedXML.Excel;
using System.Text;

namespace CinteTestNet.Application.Services;

public class ClienteService : IClienteService
{
    private readonly IUnitOfWork _unitOfWork;
    private const decimal MONTO_MINIMO_FIDELIZACION = 5000000m;

    public ClienteService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ClienteDto?> BuscarClientePorDocumentoAsync(int tipoDocumentoId, string numeroDocumento)
    {
        var cliente = await _unitOfWork.Clientes.GetByDocumentoAsync(tipoDocumentoId, numeroDocumento);
        
        if (cliente == null)
            return null;

        return MapToClienteDto(cliente);
    }

    public async Task<IEnumerable<TipoDocumentoDto>> ObtenerTiposDocumentoAsync()
    {
        var tipos = await _unitOfWork.TiposDocumento.FindAsync(t => t.Activo);
        return tipos.OrderBy(t => t.Id).Select(t => new TipoDocumentoDto
        {
            Id = t.Id,
            Nombre = t.Nombre,
            Codigo = t.Codigo
        });
    }

    public async Task<byte[]> ExportarClienteAsync(int clienteId, string formato)
    {
        var cliente = await _unitOfWork.Clientes.GetByIdWithComprasAsync(clienteId);
        
        if (cliente == null)
            throw new KeyNotFoundException("Cliente no encontrado");

        return formato.ToLower() switch
        {
            "csv" => ExportarCsv(cliente),
            "excel" => ExportarExcel(cliente),
            "txt" => ExportarTxt(cliente),
            _ => throw new ArgumentException($"Formato no soportado: {formato}")
        };
    }

    public async Task<byte[]> GenerarReporteFidelizacionAsync()
    {
        var fechaFin = DateTime.UtcNow;
        var fechaInicio = fechaFin.AddMonths(-1);

        var clientes = await _unitOfWork.Clientes.GetClientesFidelizablesAsync(
            fechaInicio, 
            fechaFin, 
            MONTO_MINIMO_FIDELIZACION
        );

        return GenerarExcelFidelizacion(clientes.ToList(), fechaInicio, fechaFin);
    }

    private ClienteDto MapToClienteDto(Cliente cliente)
    {
        var comprasCompletadas = cliente.Compras
            .Where(c => c.EstadoCompra.Codigo == "completada")
            .ToList();

        return new ClienteDto
        {
            Id = cliente.Id,
            TipoDocumento = new TipoDocumentoDto
            {
                Id = cliente.TipoDocumento.Id,
                Nombre = cliente.TipoDocumento.Nombre,
                Codigo = cliente.TipoDocumento.Codigo
            },
            NumeroDocumento = cliente.NumeroDocumento,
            Nombre = cliente.Nombre,
            Apellido = cliente.Apellido,
            Correo = cliente.Correo,
            Telefono = cliente.Telefono,
            FechaRegistro = cliente.FechaRegistro,
            Compras = cliente.Compras.Select(c => new CompraDto
            {
                Id = c.Id,
                NumeroFactura = c.NumeroFactura,
                FechaCompra = c.FechaCompra,
                Monto = c.Monto,
                Descripcion = c.Descripcion,
                Estado = c.EstadoCompra.Nombre
            }).ToList(),
            TotalCompras = comprasCompletadas.Count,
            MontoTotalCompras = comprasCompletadas.Sum(c => c.Monto)
        };
    }

    private byte[] ExportarCsv(Cliente cliente)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Campo,Valor");
        sb.AppendLine($"Tipo de Documento,{cliente.TipoDocumento.Nombre}");
        sb.AppendLine($"Número de Documento,{cliente.NumeroDocumento}");
        sb.AppendLine($"Nombre,{cliente.Nombre}");
        sb.AppendLine($"Apellido,{cliente.Apellido}");
        sb.AppendLine($"Correo,{cliente.Correo}");
        sb.AppendLine($"Teléfono,{cliente.Telefono}");
        sb.AppendLine($"Fecha de Registro,{cliente.FechaRegistro:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();
        sb.AppendLine("Compras");
        sb.AppendLine("Número Factura,Fecha,Monto,Descripción,Estado");
        
        foreach (var compra in cliente.Compras)
        {
            sb.AppendLine($"{compra.NumeroFactura},{compra.FechaCompra:yyyy-MM-dd},{compra.Monto:F2},{compra.Descripcion ?? ""},{compra.EstadoCompra.Nombre}");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private byte[] ExportarExcel(Cliente cliente)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Cliente");

        // Información del cliente
        worksheet.Cell(1, 1).Value = "Información del Cliente";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 14;

        int row = 3;
        worksheet.Cell(row, 1).Value = "Tipo de Documento:";
        worksheet.Cell(row, 2).Value = cliente.TipoDocumento.Nombre;
        row++;
        worksheet.Cell(row, 1).Value = "Número de Documento:";
        worksheet.Cell(row, 2).Value = cliente.NumeroDocumento;
        row++;
        worksheet.Cell(row, 1).Value = "Nombre:";
        worksheet.Cell(row, 2).Value = cliente.Nombre;
        row++;
        worksheet.Cell(row, 1).Value = "Apellido:";
        worksheet.Cell(row, 2).Value = cliente.Apellido;
        row++;
        worksheet.Cell(row, 1).Value = "Correo:";
        worksheet.Cell(row, 2).Value = cliente.Correo;
        row++;
        worksheet.Cell(row, 1).Value = "Teléfono:";
        worksheet.Cell(row, 2).Value = cliente.Telefono;
        row++;
        worksheet.Cell(row, 1).Value = "Fecha de Registro:";
        worksheet.Cell(row, 2).Value = cliente.FechaRegistro;

        // Compras
        row += 3;
        worksheet.Cell(row, 1).Value = "Compras";
        worksheet.Cell(row, 1).Style.Font.Bold = true;
        worksheet.Cell(row, 1).Style.Font.FontSize = 14;
        row++;

        worksheet.Cell(row, 1).Value = "Número Factura";
        worksheet.Cell(row, 2).Value = "Fecha";
        worksheet.Cell(row, 3).Value = "Monto";
        worksheet.Cell(row, 4).Value = "Descripción";
        worksheet.Cell(row, 5).Value = "Estado";
        
        var headerRange = worksheet.Range(row, 1, row, 5);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        row++;

        foreach (var compra in cliente.Compras)
        {
            worksheet.Cell(row, 1).Value = compra.NumeroFactura;
            worksheet.Cell(row, 2).Value = compra.FechaCompra;
            worksheet.Cell(row, 2).Style.DateFormat.Format = "yyyy-mm-dd";
            worksheet.Cell(row, 3).Value = compra.Monto;
            worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
            worksheet.Cell(row, 4).Value = compra.Descripcion ?? "";
            worksheet.Cell(row, 5).Value = compra.EstadoCompra.Nombre;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private byte[] ExportarTxt(Cliente cliente)
    {
        var sb = new StringBuilder();
        sb.AppendLine("INFORMACIÓN DEL CLIENTE");
        sb.AppendLine("=======================");
        sb.AppendLine($"Tipo de Documento: {cliente.TipoDocumento.Nombre}");
        sb.AppendLine($"Número de Documento: {cliente.NumeroDocumento}");
        sb.AppendLine($"Nombre: {cliente.Nombre}");
        sb.AppendLine($"Apellido: {cliente.Apellido}");
        sb.AppendLine($"Correo: {cliente.Correo}");
        sb.AppendLine($"Teléfono: {cliente.Telefono}");
        sb.AppendLine($"Fecha de Registro: {cliente.FechaRegistro:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();
        sb.AppendLine("COMPRAS");
        sb.AppendLine("=======");
        sb.AppendLine($"{"Número Factura",-20} {"Fecha",-12} {"Monto",-15} {"Estado",-15} {"Descripción"}");
        sb.AppendLine(new string('-', 100));

        foreach (var compra in cliente.Compras)
        {
            sb.AppendLine($"{compra.NumeroFactura,-20} {compra.FechaCompra:yyyy-MM-dd} {compra.Monto,15:F2} {compra.EstadoCompra.Nombre,-15} {compra.Descripcion ?? ""}");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private byte[] GenerarExcelFidelizacion(List<Cliente> clientes, DateTime fechaInicio, DateTime fechaFin)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Clientes Fidelizables");

        // Título
        worksheet.Cell(1, 1).Value = "Reporte de Fidelización de Clientes";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Range(1, 1, 1, 6).Merge();

        worksheet.Cell(2, 1).Value = $"Período: {fechaInicio:yyyy-MM-dd} a {fechaFin:yyyy-MM-dd}";
        worksheet.Cell(2, 1).Style.Font.Italic = true;
        worksheet.Range(2, 1, 2, 6).Merge();

        worksheet.Cell(3, 1).Value = $"Monto mínimo: $5,000,000.00 COP";
        worksheet.Cell(3, 1).Style.Font.Italic = true;
        worksheet.Range(3, 1, 3, 6).Merge();

        // Encabezados
        int row = 5;
        worksheet.Cell(row, 1).Value = "Tipo Documento";
        worksheet.Cell(row, 2).Value = "Número Documento";
        worksheet.Cell(row, 3).Value = "Nombre";
        worksheet.Cell(row, 4).Value = "Apellido";
        worksheet.Cell(row, 5).Value = "Correo";
        worksheet.Cell(row, 6).Value = "Teléfono";
        worksheet.Cell(row, 7).Value = "Monto Total Compras";

        var headerRange = worksheet.Range(row, 1, row, 7);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        row++;

        foreach (var cliente in clientes)
        {
            var montoTotal = cliente.Compras
                .Where(c => c.FechaCompra >= fechaInicio 
                    && c.FechaCompra <= fechaFin 
                    && c.EstadoCompra.Codigo == "completada")
                .Sum(c => c.Monto);

            worksheet.Cell(row, 1).Value = cliente.TipoDocumento.Nombre;
            worksheet.Cell(row, 2).Value = cliente.NumeroDocumento;
            worksheet.Cell(row, 3).Value = cliente.Nombre;
            worksheet.Cell(row, 4).Value = cliente.Apellido;
            worksheet.Cell(row, 5).Value = cliente.Correo;
            worksheet.Cell(row, 6).Value = cliente.Telefono;
            worksheet.Cell(row, 7).Value = montoTotal;
            worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}

