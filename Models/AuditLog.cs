using System.ComponentModel.DataAnnotations;

namespace TechRent.Models
{
    public class AuditLog
    {
        public long Id { get; set; }

        [StringLength(450)]
        public string? UserId { get; set; }

        [StringLength(256)]
        public string? Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Accion { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Entidad { get; set; }

        [StringLength(100)]
        public string? IdentificadorEntidad { get; set; }

        [StringLength(4000)]
        public string? ValorAnterior { get; set; }

        [StringLength(4000)]
        public string? ValorNuevo { get; set; }

        [StringLength(50)]
        public string? Metodo { get; set; }

        [StringLength(500)]
        public string? Ruta { get; set; }

        [StringLength(45)]
        public string? IpAddress { get; set; }

        [StringLength(500)]
        public string? UserAgent { get; set; }

        public int? StatusCode { get; set; }

        [StringLength(2000)]
        public string? Detalles { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;
    }
}
