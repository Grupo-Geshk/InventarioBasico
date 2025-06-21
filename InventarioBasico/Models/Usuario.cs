using System.ComponentModel.DataAnnotations;

namespace InventarioBasico.Models
{
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
