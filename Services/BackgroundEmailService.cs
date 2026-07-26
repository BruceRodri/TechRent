using Microsoft.EntityFrameworkCore;
using TechRent.Data;
using TechRent.Models;

namespace TechRent.Services
{
    public class BackgroundEmailService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEmailQueue _queue;
        private readonly ILogger<BackgroundEmailService> _logger;

        public BackgroundEmailService(
            IServiceProvider serviceProvider,
            IEmailQueue queue,
            ILogger<BackgroundEmailService> logger)
        {
            _serviceProvider = serviceProvider;
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BackgroundEmailService iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var item = _queue.Dequeue();

                if (item != null)
                {
                    try
                    {
                        await ProcessEmailAsync(item, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error procesando email en BackgroundEmailService");
                    }
                }
                else
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }

            _logger.LogInformation("BackgroundEmailService detenido.");
        }

        private async Task ProcessEmailAsync(EmailQueueItem item, CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var gmailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            var log = new EmailLog
            {
                Destinatario = item.Destinatario,
                Asunto = item.Asunto,
                TipoNotificacion = item.TipoNotificacion,
                FechaSolicitud = item.FechaSolicitud,
                Estado = "Enviando",
                Intentos = 0,
                ContenidoHtml = item.ContenidoHtml
            };

            context.EmailLogs.Add(log);
            await context.SaveChangesAsync(ct);

            const int maxRetries = 3;

            for (int intento = 1; intento <= maxRetries; intento++)
            {
                log.Intentos = intento;

                try
                {
                    await gmailSender.SendEmailAsync(
                        item.Destinatario,
                        item.Asunto,
                        item.ContenidoHtml);

                    log.Estado = "Enviado";
                    log.FechaEnvio = DateTime.UtcNow;
                    log.MensajeError = null;
                    await context.SaveChangesAsync(ct);

                    _logger.LogInformation(
                        "Email enviado a {Destinatario} - Tipo: {Tipo} - Intento: {Intento}",
                        item.Destinatario, item.TipoNotificacion, intento);
                    return;
                }
                catch (Exception ex)
                {
                    log.MensajeError = ex.Message;
                    _logger.LogWarning(
                        "Error enviando email a {Destinatario} - Intento {Intento}/{Max}: {Error}",
                        item.Destinatario, intento, maxRetries, ex.Message);

                    if (intento < maxRetries)
                        await Task.Delay(2000 * intento, ct);
                }
            }

            log.Estado = "Fallido";
            await context.SaveChangesAsync(ct);

            _logger.LogError(
                "Email fallido a {Destinatario} despues de {Max} intentos. Ultimo error: {Error}",
                item.Destinatario, maxRetries, log.MensajeError);
        }
    }
}
