using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using TechRent.Settings;

namespace TechRent.Services.Payments
{
    public class PayPhoneApiLinkService : IPaymentGateway
    {
        private readonly HttpClient _httpClient;
        private readonly PayPhoneSettings _settings;
        public string ProviderName => "PayPhone";

        public PayPhoneApiLinkService(HttpClient httpClient, IOptions<PayPhoneSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<PaymentStartResult> CreatePaymentAsync(PaymentRequest request)
        {
            try
            {
                var link = await CreatePaymentLinkAsync(
                    request.Monto,
                    request.ClientTransactionId ?? DateTime.Now.ToString("yyMMddHHmmssfff")[..15],
                    request.Referencia);

                return new PaymentStartResult
                {
                    Success = true,
                    ExternalTransactionId = request.ClientTransactionId,
                    ApprovalUrl = link,
                    RawResponse = link
                };
            }
            catch (Exception ex)
            {
                return new PaymentStartResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public Task<PaymentVerificationResult> VerifyPaymentAsync(string transactionId)
        {
            return Task.FromResult(new PaymentVerificationResult
            {
                Success = false,
                Status = "Pendiente",
                ErrorMessage = "PayPhone no soporta verificación por ID directo. Use el flujo de redirección."
            });
        }

        public Task<PaymentCancellationResult> CancelPaymentAsync(string transactionId)
        {
            return Task.FromResult(new PaymentCancellationResult
            {
                Success = true,
                RawResponse = $"PayPhone transacción {transactionId} cancelada localmente."
            });
        }

        public async Task<string> CreatePaymentLinkAsync(
            decimal total, string clientTransactionId, string reference)
        {
            int amountInCents = (int)Math.Round(total * 100, MidpointRounding.AwayFromZero);

            var request = new
            {
                amount = amountInCents,
                amountWithoutTax = amountInCents,
                amountWithTax = 0,
                tax = 0,
                service = 0,
                tip = 0,
                currency = "USD",
                reference = reference,
                clientTransactionId = clientTransactionId,
                additionalData = reference,
                oneTime = true,
                expireIn = 0,
                isAmountEditable = false
            };

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                "https://pay.payphonetodoesposible.com/api/Links");

            httpRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.Token);
            httpRequest.Content = JsonContent.Create(request);

            var response = await _httpClient.SendAsync(httpRequest);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"PayPhone respondió con error: {content}");

            return content.Trim('"');
        }
    }
}
