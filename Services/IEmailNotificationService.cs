namespace TechRent.Services
{
    public interface IEmailNotificationService
    {
        Task SendConfirmationEmailAsync(string email, string confirmationLink);
        Task SendPasswordResetEmailAsync(string email, string resetLink);
        Task SendPasswordResetCodeEmailAsync(string email, string resetCode);
        Task SendPasswordChangedNotificationAsync(string email);
        Task SendAccountLockedNotificationAsync(string email);
        Task SendMfaActivatedNotificationAsync(string email);
        Task SendPaymentApprovedEmailAsync(string email, int ordenId, decimal total, string detalles);
        Task SendPaymentFailedEmailAsync(string email, int ordenId, string motivo);
        Task SendCriticalInventoryAlertAsync(string adminEmail, string equipoNombre, int stockActual);
    }
}
