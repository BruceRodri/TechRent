using System.ComponentModel.DataAnnotations;
namespace TechRent.Models
{
    public class Equipo
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre del equipo es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Descripcion { get; set; }
        [Required]
        [Range(0.01, 999.99, ErrorMessage = "El precio debe estar entre 0.01 y 999.99")]
        public decimal PrecioPorDia { get; set; }
        [Range(0, 100)]
        public int Stock { get; set; }
        public string? Especificaciones { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
        // Llaves foráneas
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;
        public int MarcaId { get; set; }
        public Marca Marca { get; set; } = null!;
        // Relación: un equipo puede estar en muchos detalles de reserva
        public ICollection<DetalleReserva> DetalleReservas { get; set; } = new List<DetalleReserva>();
        public DateTime? FechaEliminacion { get; set; }
    }
}