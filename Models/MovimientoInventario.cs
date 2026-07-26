using System.ComponentModel.DataAnnotations;
namespace TechRent.Models
{
    public class MovimientoInventario
    {
        public int Id { get; set; }

        [Required]
        public int EquipoId { get; set; }
        public Equipo Equipo { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string TipoMovimiento { get; set; } = string.Empty;

        [Required]
        public int Cantidad { get; set; }

        public int StockAnterior { get; set; }

        public int StockPosterior { get; set; }

        [StringLength(200)]
        public string? Referencia { get; set; }

        public DateTime FechaMovimiento { get; set; } = DateTime.UtcNow;

        [StringLength(450)]
        public string? UsuarioId { get; set; }

        [StringLength(500)]
        public string? Observacion { get; set; }
    }
}
