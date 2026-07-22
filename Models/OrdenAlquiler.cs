namespace TechRent.Models
{
    public class OrdenAlquiler
    {
        public int Id { get; set; }
        public string UsuarioEmail { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaPago { get; set; }
        public ICollection<DetalleOrdenAlquiler> Detalles { get; set; } = new List<DetalleOrdenAlquiler>();
        public ICollection<TransaccionPago> Transacciones { get; set; } = new List<TransaccionPago>();
    }
}
