using System.ComponentModel.DataAnnotations;
namespace TechRent.Models
{
    public class Marca
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [StringLength(100)]
        public string? PaisOrigen { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
        // Relación: una marca tiene muchos equipos
        public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
    }
}