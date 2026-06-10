using System.ComponentModel.DataAnnotations;

namespace TechRent.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Range(0, 99999.99)]
        public decimal MontoTotal { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }

        // Llaves foráneas
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        public int EstadoReservaId { get; set; }
        public EstadoReserva EstadoReserva { get; set; } = null!;

        // Relaciones: una reserva tiene muchos detalles y muchos pagos
        public ICollection<DetalleReserva> DetalleReservas { get; set; } = new List<DetalleReserva>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}