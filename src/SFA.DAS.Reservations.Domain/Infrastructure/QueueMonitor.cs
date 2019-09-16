namespace SFA.DAS.Reservations.Domain.Infrastructure
{
    public class QueueMonitor
    {
        public string QueueName { get; }
        public bool? IsHealthy { get; set; }

        public QueueMonitor(string queueName, bool? isHealthy)
        {
            QueueName = queueName;
            IsHealthy = isHealthy;
        }
    }
}
