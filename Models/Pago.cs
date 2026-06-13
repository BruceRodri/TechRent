using System.ComponentModel.DataAnnotations;

namespace TechRent.Models
{
    public class Pago
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01, 99999.99)]
        public decimal Monto { get; set; }

        [Required]
        public DateTime FechaPago { get; set; }

        [Required]
        [StringLength(50)]
        public string MetodoPago { get; set; } = string.Empty; // Efectivo, Tarjeta, Transferencia

        [StringLength(100)]
        public string? ReferenciaPago { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

        // Llave foránea
        public int ReservaId { get; set; }
        public Reserva Reserva { get; set; } = null!;
    }
}