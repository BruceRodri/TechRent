namespace TechRent.Services
{
    public interface IEmailQueue
    {
        void Enqueue(EmailQueueItem item);
        EmailQueueItem? Dequeue();
        int Count { get; }
    }

    public class EmailQueueItem
    {
        public string Destinatario { get; set; } = string.Empty;
        public string Asunto { get; set; } = string.Empty;
        public string TipoNotificacion { get; set; } = string.Empty;
        public string ContenidoHtml { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
    }
}
