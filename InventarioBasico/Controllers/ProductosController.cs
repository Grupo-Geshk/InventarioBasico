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
    [Route("productos")]
    public class ProductosController : ControllerBase
    {
        private readonly InventarioDbContext _context;
        private readonly ExcelExportService _excelService;

        public ProductosController(InventarioDbContext context, ExcelExportService excelService)
        {
            _context = context;
            _excelService = excelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productos = await _context.Productos.Include(p => p.Categoria)
                .Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    StockActual = p.StockActual,
                    PrecioUnitario = p.PrecioUnitario,
                    ImagenUrl = p.ImagenUrl,
                    CategoriaNombre = p.Categoria.Nombre
                })
                .ToListAsync();

            return Ok(productos);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var p = await _context.Productos.Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p == null) return NotFound();

            var productoDto = new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                StockActual = p.StockActual,
                PrecioUnitario = p.PrecioUnitario,
                ImagenUrl = p.ImagenUrl,
                CategoriaNombre = p.Categoria.Nombre
            };

            return Ok(productoDto);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductoCreateDto dto)
        {
            var producto = new Producto
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                StockActual = dto.StockActual,
                PrecioUnitario = dto.PrecioUnitario,
                ImagenUrl = dto.ImagenUrl,
                CategoriaId = dto.CategoriaId
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return Ok(new { producto.Id });
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductoCreateDto dto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.StockActual = dto.StockActual;
            producto.PrecioUnitario = dto.PrecioUnitario;
            producto.ImagenUrl = dto.ImagenUrl;
            producto.CategoriaId = dto.CategoriaId;

            await _context.SaveChangesAsync();

            return Ok(new { producto.Id });
        }

        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel()
        {
            var bytes = await _excelService.ExportProductosToExcel();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "productos.xlsx");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
