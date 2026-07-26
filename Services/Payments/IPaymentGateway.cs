namespace TechRent.Services.Payments
{
    public interface IPaymentGateway
    {
        string ProviderName { get; }
        Task<PaymentStartResult> CreatePaymentAsync(PaymentRequest request);
        Task<PaymentVerificationResult> VerifyPaymentAsync(string transactionId);
        Task<PaymentCancellationResult> CancelPaymentAsync(string transactionId);
    }

    public class PaymentRequest
    {
        public int OrdenAlquilerId { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; } = "USD";
        public string Referencia { get; set; } = string.Empty;
        public string? ClientTransactionId { get; set; }
    }

    public class PaymentStartResult
    {
        public bool Success { get; set; }
        public string? ExternalTransactionId { get; set; }
        public string? ApprovalUrl { get; set; }
        public string? RawResponse { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class PaymentVerificationResult
    {
        public bool Success { get; set; }
        public string Status { get; set; } = "Pendiente";
        public string? CaptureId { get; set; }
        public string? RawResponse { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class PaymentCancellationResult
    {
        public bool Success { get; set; }
        public string? RawResponse { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
