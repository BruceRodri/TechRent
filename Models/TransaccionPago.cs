namespace TechRent.Models
{
    public class TransaccionPago
    {
        public int Id { get; set; }
        public int OrdenAlquilerId { get; set; }
        public OrdenAlquiler OrdenAlquiler { get; set; } = null!;
        public string Proveedor { get; set; } = "PayPal";
        public string? ClientTransactionId { get; set; }
        public string? PayphonePaymentUrl { get; set; }
        public string? PayPalOrderId { get; set; }
        public string? PayPalCaptureId { get; set; }
        public string? PayPalApprovalUrl { get; set; }
        public int MontoEnCentavos { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public string? RespuestaGateway { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaConfirmacion { get; set; }
    }
}
