using InventarioBasico.Data;
using InventarioBasico.Dto;
using InventarioBasico.Models;
using InventarioBasico.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioBasico.Controllers
{
    [ApiController]
    [Route("movimientos")]
    public class MovimientosController : ControllerBase
    {
        private readonly InventarioDbContext _context;
        private readonly EmailService _emailService;
        private readonly ExcelExportService _excelService;

        public MovimientosController(InventarioDbContext context, EmailService emailService, ExcelExportService excelService)
        {
            _context = context;
            _emailService = emailService;
            _excelService = excelService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movimientos = await _context.MovimientosInventario.Include(m => m.Producto).ToListAsync();
            return Ok(movimientos);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MovimientoCreateDto dto)
        {
            var producto = await _context.Productos.FindAsync(dto.ProductoId);
            if (producto == null) return BadRequest("Producto no encontrado");

            // Ajustar stock
            switch ((TipoMovimiento)dto.TipoMovimiento)
            {
                case TipoMovimiento.Ingreso:
                    producto.StockActual += dto.Cantidad;
                    break;
                case TipoMovimiento.Salida:
                    producto.StockActual -= dto.Cantidad;
                    break;
                case TipoMovimiento.Ajuste:
                    producto.StockActual = dto.Cantidad;
                    break;
            }

            var movimiento = new MovimientoInventario
            {
                ProductoId = dto.ProductoId,
                Cantidad = dto.Cantidad,
                Notas = dto.Notas,
                TipoMovimiento = (TipoMovimiento)dto.TipoMovimiento
            };

            _context.MovimientosInventario.Add(movimiento);
            await _context.SaveChangesAsync();

            // Verificar si el stock quedó bajo
            if (producto.StockActual < 10)
            {
                var toEmail = "destino@correo.com"; // CAMBIA esto por tu email real
                var subject = $"⚠️ Stock bajo: {producto.Nombre}";
                var body = $"El producto <b>{producto.Nombre}</b> tiene stock bajo: {producto.StockActual} unidades.";

                await _emailService.SendEmailAsync(toEmail, subject, body);
            }

            return Ok(new { movimiento.Id });
        }
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel()
        {
            var bytes = await _excelService.ExportMovimientosToExcel();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "movimientos.xlsx");
        }
    }
}
