using System.Collections.Concurrent;

namespace TechRent.Services
{
    public class EmailQueue : IEmailQueue
    {
        private readonly ConcurrentQueue<EmailQueueItem> _queue = new();

        public void Enqueue(EmailQueueItem item)
        {
            _queue.Enqueue(item);
        }

        public EmailQueueItem? Dequeue()
        {
            return _queue.TryDequeue(out var item) ? item : null;
        }

        public int Count => _queue.Count;
    }
}
