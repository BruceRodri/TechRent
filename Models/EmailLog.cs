using System.ComponentModel.DataAnnotations;

namespace TechRent.Models
{
    public class EmailLog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Destinatario { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string Asunto { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TipoNotificacion { get; set; } = string.Empty;

        public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;

        public DateTime? FechaEnvio { get; set; }

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Pendiente";

        public int Intentos { get; set; } = 0;

        [StringLength(1000)]
        public string? MensajeError { get; set; }

        [StringLength(2000)]
        public string? ContenidoHtml { get; set; }
    }
}
