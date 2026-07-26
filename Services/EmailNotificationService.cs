namespace TechRent.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IEmailQueue _queue;

        public EmailNotificationService(IEmailQueue queue)
        {
            _queue = queue;
        }

        public Task SendConfirmationEmailAsync(string email, string confirmationLink)
        {
            _queue.Enqueue(new EmailQueueItem
            {
                Destinatario = email,
                Asunto = "Confirma tu correo en TechRent",
                TipoNotificacion = "ConfirmacionCorreo",
                ContenidoHtml = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
                    <div style='background:#4361ee;color:white;padding:20px;text-align:center;'>
                        <h1 style='margin:0;'>TechRent</h1>
                        <p style='margin:5px 0 0;'>Confirmacion de correo</p>
                    </div>
                    <div style='padding:20px;border:1px solid #ddd;'>
                        <h2 style='color:#4361ee;'>Confirma tu correo electronico</h2>
                        <p>Haz clic en el siguiente enlace para confirmar tu cuenta:</p>
                        <a href='{confirmationLink}' style='display:inline-block;padding:12px 24px;background:#4361ee;color:white;text-decoration:none;border-radius:8px;font-weight:bold;margin:15px 0;'>Confirmar correo</a>
                        <p style='color:#666;font-size:0.9em;'>Si no solicitaste esta cuenta, puedes ignorar este mensaje.</p>
                    </div>
                    <div style='text-align:center;padding:10px;color:#999;font-size:0.8em;'>
                        TechRent - Sistema de Alquiler de Equipos Tecnologicos
                    </div>
                </div>"
            });
            return Task.CompletedTask;
        }

        public Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            _queue.Enqueue(new EmailQueueItem
            {
                Destinatario = email,
                Asunto = "Restablece tu contrasena en TechRent",
                TipoNotificacion = "RecuperacionContrasena",
                ContenidoHtml = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
                    <div style='background:#e74c3c;color:white;padding:20px;text-align:center;'>
                        <h1 style='margin:0;'>TechRent</h1>
                        <p style='margin:5px 0 0;'>Recuperacion de contrasena</p>
                    </div>
                    <div style='padding:20px;border:1px solid #ddd;'>
                        <h2 style='color:#e74c3c;'>Restablece tu contrasena</h2>
                        <p>Haz clic en el siguiente enlace para restablecer tu contrasena:</p>
                        <a href='{resetLink}' style='display:inline-block;padding:12px 24px;background:#e74c3c;color:white;text-decoration:none;border-radius:8px;font-weight:bold;margin:15px 0;'>Restablecer contrasena</a>
                        <p style='color:#666;font-size:0.9em;'>Este enlace expirara en 24 horas. Si no solicitaste este cambio, ignora este mensaje.</p>
                    </div>
                    <div style='text-align:center;padding:10px;color:#999;font-size:0.8em;'>
                        TechRent - Sistema de Alquiler de Equipos Tecnologicos
                    </div>
                </div>"
            });
            return Task.CompletedTask;
        }

        public Task SendPasswordResetCodeEmailAsync(string email, string resetCode)
        {
            _queue.Enqueue(new EmailQueueItem
            {
                Destinatario = email,
                Asunto = "Codigo de restablecimiento en TechRent",
                TipoNotificacion = "CodigoRecuperacion",
                ContenidoHtml = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
                    <div style='background:#f39c12;color:white;padding:20px;text-align:center;'>
                        <h1 style='margin:0;'>TechRent</h1>
                        <p style='margin:5px 0 0;'>Codigo de verificacion</p>
                    </div>
                    <div style='padding:20px;border:1px solid #ddd;text-align:center;'>
                        <h2 style='color:#f39c12;'>Tu codigo es:</h2>
                        <div style='font-size:2em;font-weight:bold;letter-spacing:5px;color:#2c3e50;margin:20px 0;'>{resetCode}</div>
                        <p style='color:#666;font-size:0.9em;'>Este codigo expirara en 10 minutos.</p>
                    </div>
                    <div style='text-align:center;padding:10px;color:#999;font-size:0.8em;'>
                        TechRent - Sistema de Alquiler de Equipos Tecnologicos
                    </div>
                </div>"
            });
            return Task.CompletedTask;
        }

        public Task SendPasswordChangedNotificationAsync(string email)
        {
            _queue.Enqueue(new EmailQueueItem
            {
                Destinatario = email,
                Asunto = "Tu contrasena fue cambiada en TechRent",
                TipoNotificacion = "CambioContrasena",
                ContenidoHtml = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
                    <div style='background:#27ae60;color:white;padding:20px;text-align:center;'>
                        <h1 style='margin:0;'>TechRent</h1>
                        <p style='margin:5px 0 0;'>Notificacion de seguridad</p>
                    </div>
                    <div style='padding:20px;border:1px solid #ddd;'>
                        <h2 style='color:#27ae60;'>Contrasena actualizada</h2>
                        <p>Tu contrasena fue cambiada exitosamente el <strong>{DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC</strong>.</p>
                        <p style='color:#666;font-size:0.9em;'>Si no realizaste este cambio, contacta soporte inmediatamente.</p>
                    </div>
                    <div style='text-align:center;padding:10px;color:#999;font-size:0.8em;'>
                        TechRent - Sistema de Alquiler de Equipos Tecnologicos
                    </div>
                </div>"
            });
            return Task.CompletedTask;
        }

        public Task SendAccountLockedNotificationAsync(string email)
        {
            _queue.Enqueue(new EmailQueueItem
            {
                Destinatario = email,
                Asunto = "Tu cuenta fue bloqueada en TechRent",
                TipoNotificacion = "CuentaBloqueada",
                ContenidoHtml = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
                    <div style='background:#c0392b;color:white;padding:20px;text-align:center;'>
                        <h1 style='margin:0;'>TechRent</h1>
                        <p style='margin:5px 0 0;'>Alerta de seguridad</p>
                    </div>
                    <div style='padding:20px;border:1px solid #ddd;'>
                        <h2 style='color:#c0392b;'>Cuenta bloqueada</h2>
                        <p>Tu cuenta fue bloqueada por seguridad despues de multiples intentos fallidos de inicio de sesion.</p>
                        <p><strong>Fecha:</strong> {DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC</p>
                        <p style='color:#666;font-size:0.9em;'>Si no fuiste tu, contacta soporte inmediatamente.</p>
                    </div>
                    <div style='text-align:center;padding:10px;color:#999;font-size:0.8em;'>
                        TechRent - Sistema de Alquiler de Equipos Tecnologicos
                    </div>
                </div>"
            });
            return Task.CompletedTask;
        }

        public Task SendMfaActivatedNotificationAsync(string email)
        {
            _queue.Enqueue(new EmailQueueItem
            {
                Destinatario = email,
                Asunto = "Autenticacion de dos factores activada en TechRent",
                TipoNotificacion = "MFAActivado",
                ContenidoHtml = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
                    <div style='background:#8e44ad;color:white;padding:20px;text-align:center;'>
                        <h1 style='margin:0;'>TechRent</h1>
                        <p style='margin:5px 0 0;'>Seguridad de cuenta</p>
                    </div>
                    <div style='padding:20px;border:1px solid #ddd;'>
                        <h2 style='color:#8e44ad;'>MFA activado</h2>
                        <p>La autenticacion de dos factores fue activada exitosamente en tu cuenta.</p>
                        <p><strong>Fecha:</strong> {DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC</p>
                        <p style='color:#666;font-size:0.9em;'>Ahora necesitaras tu dispositivo de autenticacion para iniciar sesion.</p>
                    </div>
                    <div style='text-align:center;padding:10px;color:#999;font-size:0.8em;'>
                        TechRent - Sistema de Alquiler de Equipos Tecnologicos
                    </div>
                </div>"
            });
            return Task.CompletedTask;
        }

        public Task SendPaymentApprovedEmailAsync(string email, int ordenId, decimal total, string detalles)
        {
            _queue.Enqueue(new EmailQueueItem
            {
                Destinatario = email,
                Asunto = $"TechRent - Pago aprobado Orden #{ordenId}",
                TipoNotificacion = "VentaAprobada",
                ContenidoHtml = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
                    <div style='background:#27ae60;color:white;padding:20px;text-align:center;'>
                        <h1 style='margin:0;'>TechRent</h1>
                        <p style='margin:5px 0 0;'>Confirmacion de pago</p>
                    </div>
                    <div style='padding:20px;border:1px solid #ddd;'>
                        <h2 style='color:#27ae60;'>Pago aprobado</h2>
                        <p>Tu pago fue procesado exitosamente.</p>
                        <p><strong>Orden:</strong> #{ordenId}</p>
                        <p><strong>Total:</strong> <span style='color:#27ae60;font-size:1.2em;'>${total:F2}</span></p>
                        {detalles}
                        <p style='color:#666;font-size:0.9em;'>Gracias por confiar en TechRent.</p>
                    </div>
                    <div style='text-align:center;padding:10px;color:#999;font-size:0.8em;'>
                        TechRent - Sistema de Alquiler de Equipos Tecnologicos
                    </div>
                </div>"
            });
            return Task.CompletedTask;
        }

        public Task SendPaymentFailedEmailAsync(string email, int ordenId, string motivo)
        {
            _queue.Enqueue(new EmailQueueItem
            {
                Destinatario = email,
                Asunto = $"TechRent - Pago fallido Orden #{ordenId}",
                TipoNotificacion = "PagoFallido",
                ContenidoHtml = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
                    <div style='background:#e74c3c;color:white;padding:20px;text-align:center;'>
                        <h1 style='margin:0;'>TechRent</h1>
                        <p style='margin:5px 0 0;'>Notificacion de pago</p>
                    </div>
                    <div style='padding:20px;border:1px solid #ddd;'>
                        <h2 style='color:#e74c3c;'>Pago fallido</h2>
                        <p>Tu pago para la orden <strong>#{ordenId}</strong> no pudo ser procesado.</p>
                        <p><strong>Motivo:</strong> {motivo}</p>
                        <p style='color:#666;font-size:0.9em;'>Puedes intentar nuevamente desde tu carrito de compras.</p>
                    </div>
                    <div style='text-align:center;padding:10px;color:#999;font-size:0.8em;'>
                        TechRent - Sistema de Alquiler de Equipos Tecnologicos
                    </div>
                </div>"
            });
            return Task.CompletedTask;
        }

        public Task SendCriticalInventoryAlertAsync(string adminEmail, string equipoNombre, int stockActual)
        {
            _queue.Enqueue(new EmailQueueItem
            {
                Destinatario = adminEmail,
                Asunto = $"TechRent - Alerta de inventario critico: {equipoNombre}",
                TipoNotificacion = "InventarioCritico",
                ContenidoHtml = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:0 auto;'>
                    <div style='background:#f39c12;color:white;padding:20px;text-align:center;'>
                        <h1 style='margin:0;'>TechRent</h1>
                        <p style='margin:5px 0 0;'>Alerta de inventario</p>
                    </div>
                    <div style='padding:20px;border:1px solid #ddd;'>
                        <h2 style='color:#f39c12;'>Stock critico</h2>
                        <p>El equipo <strong>{equipoNombre}</strong> tiene stock critico.</p>
                        <p><strong>Stock actual:</strong> <span style='color:#e74c3c;font-size:1.2em;'>{stockActual} unidades</span></p>
                        <p style='color:#666;font-size:0.9em;'>Se recomienda reabastecer este equipo lo antes posible.</p>
                    </div>
                    <div style='text-align:center;padding:10px;color:#999;font-size:0.8em;'>
                        TechRent - Sistema de Alquiler de Equipos Tecnologicos
                    </div>
                </div>"
            });
            return Task.CompletedTask;
        }
    }
}
