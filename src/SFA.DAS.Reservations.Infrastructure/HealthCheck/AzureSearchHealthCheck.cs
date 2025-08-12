using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.Reservations.Domain.Reservations;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Infrastructure.HealthCheck;

public class AzureSearchHealthCheck(IAzureSearchReservationIndexRepository azureSearchReservationIndexRepository) : IHealthCheck
{
    private const string HealthCheckResultDescription = "Azure search re-indexing health";

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await azureSearchReservationIndexRepository.GetHealthCheckStatus(cancellationToken);

        return result == Domain.Models.HealthCheckResult.Healthy
            ? HealthCheckResult.Healthy(HealthCheckResultDescription)
            : HealthCheckResult.Degraded(HealthCheckResultDescription);
    }
}
