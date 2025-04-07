namespace SFA.DAS.Reservations.Domain.Infrastructure
{
    public class QueueMonitor(string queueName, bool? isHealthy)
    {
        public string QueueName { get; } = queueName;
        public bool? IsHealthy { get; set; } = isHealthy;
    }
}
