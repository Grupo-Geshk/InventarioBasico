using ClosedXML.Excel;
using InventarioBasico.Data;
using Microsoft.EntityFrameworkCore;

namespace InventarioBasico.Services
{
    public class ExcelExportService
    {
        private readonly InventarioDbContext _context;

        public ExcelExportService(InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> ExportProductosToExcel()
        {
            var productos = await _context.Productos.Include(p => p.Categoria).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Productos");

            worksheet.Cell(1, 1).Value = "Nombre";
            worksheet.Cell(1, 2).Value = "Descripción";
            worksheet.Cell(1, 3).Value = "Stock Actual";
            worksheet.Cell(1, 4).Value = "Precio Unitario";
            worksheet.Cell(1, 5).Value = "Categoría";

            int row = 2;
            foreach (var p in productos)
            {
                worksheet.Cell(row, 1).Value = p.Nombre;
                worksheet.Cell(row, 2).Value = p.Descripcion;
                worksheet.Cell(row, 3).Value = p.StockActual;
                worksheet.Cell(row, 4).Value = p.PrecioUnitario;
                worksheet.Cell(row, 5).Value = p.Categoria?.Nombre ?? "";
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        public async Task<byte[]> ExportMovimientosToExcel()
        {
            var movimientos = await _context.MovimientosInventario
                .Include(m => m.Producto)
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Movimientos");

            worksheet.Cell(1, 1).Value = "Fecha";
            worksheet.Cell(1, 2).Value = "Producto";
            worksheet.Cell(1, 3).Value = "Tipo";
            worksheet.Cell(1, 4).Value = "Cantidad";
            worksheet.Cell(1, 5).Value = "Notas";

            int row = 2;
            foreach (var m in movimientos)
            {
                worksheet.Cell(row, 1).Value = m.FechaMovimiento.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 2).Value = m.Producto?.Nombre ?? "N/A";
                worksheet.Cell(row, 3).Value = m.TipoMovimiento.ToString();
                worksheet.Cell(row, 4).Value = m.Cantidad;
                worksheet.Cell(row, 5).Value = m.Notas ?? "";
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

    }
}
