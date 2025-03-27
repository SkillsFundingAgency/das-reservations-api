using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Infrastructure.AzureServiceBus
{
    public class AzureQueueService(IOptions<ReservationsConfiguration> options) : IAzureQueueService
    {
        private readonly ReservationsConfiguration _configuration = options.Value;

        public IList<QueueMonitor> GetQueuesToMonitor()
        {
            var queuesToMonitor = _configuration
                .QueueMonitorItems.Split(',')
                .Select(c => new QueueMonitor(c, null))
                .ToList();
            
            return queuesToMonitor;
        }

        public async Task<bool> IsQueueHealthy(string queueName)
        {
            var connectionString = new ServiceBusConnectionStringBuilder(_configuration.NServiceBusConnectionString);
            var tokenProvider = TokenProvider.CreateManagedIdentityTokenProvider();
            var client = new ManagementClient(connectionString, tokenProvider);

            if (!await client.QueueExistsAsync(queueName))
            {
                return false;
            }

            var queue = await client.GetQueueRuntimeInfoAsync(queueName);

            return queue.MessageCount == 0;
        }

       
    }
}
