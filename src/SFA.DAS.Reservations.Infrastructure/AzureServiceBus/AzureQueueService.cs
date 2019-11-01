using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Infrastructure.AzureServiceBus
{
    public class AzureQueueService : IAzureQueueService
    {
        private readonly ReservationsConfiguration _configuration;

        public AzureQueueService(IOptions<ReservationsConfiguration> options)
        {
            _configuration = options.Value;
        }

        public async Task<IList<QueueMonitor>> GetQueuesToMonitor()
        {
            var queueNames = _configuration.QueueMonitorItems.Split(',');

            var client = new ManagementClient(_configuration.NServiceBusConnectionString);

            var validQueueNames = new List<string>();

            foreach (var name in queueNames)
            {
                if (await client.QueueExistsAsync(name))
                {
                    validQueueNames.Add(name);
                }
            }

            var queuesToMonitor = validQueueNames
                    .Select(c => new QueueMonitor(c, null))
                    .ToList();
            
            return queuesToMonitor;
        }

        public async Task<bool> IsQueueHealthy(string queueName)
        {
            var client = new ManagementClient(_configuration.NServiceBusConnectionString);

            if (!await client.QueueExistsAsync(queueName))
            {
                return false;
            }

            var queue = await client.GetQueueRuntimeInfoAsync(queueName);

            return queue.MessageCount == 0;
        }

       
    }
}
