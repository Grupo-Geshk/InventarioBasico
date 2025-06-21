using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioBasico.Models
{
    public class Producto
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string Nombre { get; set; }

        [MaxLength(1000)]
        public string? Descripcion { get; set; }

        public int StockActual { get; set; }

        public decimal PrecioUnitario { get; set; }

        [MaxLength(500)]
        public string? ImagenUrl { get; set; }

        [ForeignKey("Categoria")]
        public Guid CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
