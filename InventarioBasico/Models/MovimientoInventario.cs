using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioBasico.Models
{
    public enum TipoMovimiento
    {
        Ingreso = 1,
        Salida = 2,
        Ajuste = 3
    }

    public class MovimientoInventario
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [ForeignKey("Producto")]
        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; }

        [Required]
        public TipoMovimiento TipoMovimiento { get; set; }

        public int Cantidad { get; set; }

        public string? Notas { get; set; }

        public DateTime FechaMovimiento { get; set; } = DateTime.UtcNow;
    }
}
