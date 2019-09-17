﻿using System.Collections.Generic;
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
            var client = new ManagementClient(_configuration.NServiceBusConnectionString);
            var queue = await client.GetQueueRuntimeInfoAsync(queueName);

            return queue.MessageCount == 0;
        }

       
    }
}
