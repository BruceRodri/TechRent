using System.ComponentModel.DataAnnotations;

namespace TechRent.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Formato de email no válido")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone(ErrorMessage = "Formato de teléfono no válido")]
        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(20)]
        public string? DocumentoIdentidad { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        // Relación: un cliente puede tener muchas reservas
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        public DateTime? FechaEliminacion { get; set; }
    }
}