using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Infrastructure.HealthCheck
{
    public class QueueHealthCheck : IHealthCheck
    {
        private const string HealthCheckResultDescription = "Reservation ServiceBus Queue check";
        private readonly IAzureQueueService _azureQueueService;

        public QueueHealthCheck(IAzureQueueService azureQueueService)
        {
            _azureQueueService = azureQueueService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var timer = Stopwatch.StartNew();
            var queues = _azureQueueService.GetQueuesToMonitor();

            foreach (var queue in queues)
            {
                var queueStatus = await _azureQueueService.IsQueueHealthy(queue.QueueName);

                if (queue.IsHealthy.HasValue && queueStatus == queue.IsHealthy)
                {
                    continue;
                }

                queues[queues.IndexOf(queue)].IsHealthy = queueStatus;
            }

            timer.Stop();

            var durationString = timer.Elapsed.ToHumanReadableString();

            if (queues.All(c => c.IsHealthy.HasValue && c.IsHealthy.Value))
            {
                return HealthCheckResult.Healthy(HealthCheckResultDescription,
                    new Dictionary<string, object> {{"Duration", durationString}});
            }

            var errorDataDictionary = new Dictionary<string, object>
            {
                {"Duration", durationString},
                {"QueuesInError", queues
                    .Where(c=>c.IsHealthy == false)
                    .Select(c => c.QueueName)
                    .Aggregate((item1, item2) => item1 + ", " + item2)}
            };

            return new HealthCheckResult(
                queues.All(c => c.IsHealthy.HasValue && !c.IsHealthy.Value) ? HealthStatus.Unhealthy : HealthStatus.Degraded,
                HealthCheckResultDescription, null, errorDataDictionary );

        }
    }
}