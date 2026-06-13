using System.ComponentModel.DataAnnotations;

namespace TechRent.Models
{
    public class DetalleReserva
    {
        public int Id { get; set; }

        [Required]
        [Range(1, 100)]
        public int Cantidad { get; set; }

        [Required]
        [Range(0.01, 999.99)]
        public decimal PrecioUnitarioPorDia { get; set; }

        [Required]
        [Range(1, 365)]
        public int CantidadDias { get; set; }

        [Range(0.01, 99999.99)]
        public decimal Subtotal { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Llaves foráneas
        public int ReservaId { get; set; }
        public Reserva Reserva { get; set; } = null!;

        public int EquipoId { get; set; }
        public Equipo Equipo { get; set; } = null!;
        public DateTime? FechaEliminacion { get; set; }
    }
}