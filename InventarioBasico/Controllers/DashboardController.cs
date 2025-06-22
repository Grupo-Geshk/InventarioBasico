using InventarioBasico.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioBasico.Controllers
{
    [ApiController]
    [Route("dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public DashboardController(InventarioDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalProductos = await _context.Productos.CountAsync();
            var productosBajoStock = await _context.Productos.Where(p => p.StockActual < 10).ToListAsync();
            var totalMovimientos = await _context.MovimientosInventario.CountAsync();

            return Ok(new
            {
                TotalProductos = totalProductos,
                BajoStock = productosBajoStock,
                TotalMovimientos = totalMovimientos
            });
        }
    }
}
