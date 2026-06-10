using System.ComponentModel.DataAnnotations;

namespace TechRent.Models
{
    public class EstadoReserva
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty; // Pendiente, Activa, Completada, Cancelada

        [StringLength(200)]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación: un estado puede tener muchas reservas
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}