using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using TechRent.Settings;

namespace TechRent.Services
{
    public class GmailEmailSender : IEmailSender<IdentityUser>, TechRent.Services.IEmailSender
    {
        private readonly EmailSettings _settings;

        public GmailEmailSender(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendEmailAsync(IdentityUser user, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(user.Email));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlMessage }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public Task SendConfirmationLinkAsync(IdentityUser user, string email, string confirmationLink)
        {
            return SendEmailAsync(user, "Confirma tu correo en TechRent",
                $"Por favor confirma tu cuenta haciendo <a href='{confirmationLink}'>clic aqui</a>.");
        }

        public Task SendPasswordResetLinkAsync(IdentityUser user, string email, string resetLink)
        {
            return SendEmailAsync(user, "Restablece tu contrasena en TechRent",
                $"Haz clic en el siguiente enlace para restablecer tu contrasena: <a href='{resetLink}'>Restablecer contrasena</a>.");
        }

        public Task SendPasswordResetCodeAsync(IdentityUser user, string email, string resetCode)
        {
            return SendEmailAsync(user, "Codigo de restablecimiento en TechRent",
                $"Tu codigo de restablecimiento es: <strong>{resetCode}</strong>");
        }
    }
}
