using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Infrastructure
{
    public interface IAzureQueueService
    {
        IList<QueueMonitor> GetQueuesToMonitor();
        Task<bool> IsQueueHealthy(string expectedQueueName);
    }
}
