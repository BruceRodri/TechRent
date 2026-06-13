using System.ComponentModel.DataAnnotations;
namespace TechRent.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
        // Relación: una categoría tiene muchos equipos
        public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
        public DateTime? FechaEliminacion { get; set; }
    }
}