using InventarioBasico.Data;
using InventarioBasico.Dto;
using InventarioBasico.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioBasico.Controllers
{
    [ApiController]
    [Route("categorias")]
    public class CategoriasController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public CategoriasController(InventarioDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categorias = await _context.Categorias
                .Select(c => new CategoriaDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    FechaCreacion = c.FechaCreacion
                })
                .ToListAsync();

            return Ok(categorias);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoriaCreateDto dto)
        {
            var categoria = new Categoria
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
                // Id y FechaCreacion se generan solos
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return Ok(new CategoriaDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                FechaCreacion = categoria.FechaCreacion
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CategoriaCreateDto dto)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            categoria.Nombre = dto.Nombre;
            categoria.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();

            return Ok(new CategoriaDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                FechaCreacion = categoria.FechaCreacion
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
