using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Infrastructure.HealthCheck
{
    public class ElasticSearchHealthCheck : IHealthCheck
    {
        private readonly IReservationIndexRepository _repository;


        public ElasticSearchHealthCheck(IReservationIndexRepository repository)
        {
            _repository = repository;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            var elasticInstanceOnline = await _repository.PingAsync();

            if (!elasticInstanceOnline)
            {
                return HealthCheckResult.Unhealthy("Ping to elastic instance failed");
            }

            var latestIndex = await _repository.GetCurrentReservationIndex();

            if (latestIndex == null)
            {
                return HealthCheckResult.Unhealthy("There are no available indices");
            }

            if (latestIndex.DateCreated < DateTime.Now.AddDays(-1))
            {
                return HealthCheckResult.Degraded("Latest index is more than one day old");
            }

            return HealthCheckResult.Healthy("All elastic search checks have passed");
        }
    }
}
