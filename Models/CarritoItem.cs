using System.ComponentModel.DataAnnotations;

namespace TechRent.Models
{
    public class CarritoItem
    {
        public int Id { get; set; }

        [Required]
        public string UsuarioEmail { get; set; } = string.Empty;

        public int EquipoId { get; set; }
        public Equipo Equipo { get; set; } = null!;

        [Range(1, 100)]
        public int Cantidad { get; set; } = 1;

        [Range(1, 365)]
        public int Dias { get; set; } = 1;

        public DateTime FechaAgregado { get; set; } = DateTime.UtcNow;
    }
}
