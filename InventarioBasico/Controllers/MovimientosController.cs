using InventarioBasico.Data;
using InventarioBasico.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioBasico.Controllers
{
    [ApiController]
    [Route("movimientos")]
    [Authorize]
    public class MovimientosController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public MovimientosController(InventarioDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movimientos = await _context.MovimientosInventario.Include(m => m.Producto).ToListAsync();
            return Ok(movimientos);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MovimientoInventario movimiento)
        {
            var producto = await _context.Productos.FindAsync(movimiento.ProductoId);
            if (producto == null) return BadRequest("Producto no encontrado");

            // Ajustar stock
            switch (movimiento.TipoMovimiento)
            {
                case TipoMovimiento.Ingreso:
                    producto.StockActual += movimiento.Cantidad;
                    break;
                case TipoMovimiento.Salida:
                    producto.StockActual -= movimiento.Cantidad;
                    break;
                case TipoMovimiento.Ajuste:
                    producto.StockActual = movimiento.Cantidad;
                    break;
            }

            _context.MovimientosInventario.Add(movimiento);
            await _context.SaveChangesAsync();

            return Ok(movimiento);
        }
    }
}
