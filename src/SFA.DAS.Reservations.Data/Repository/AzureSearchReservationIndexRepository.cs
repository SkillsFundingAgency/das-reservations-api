using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.Constants;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.Models;
using SFA.DAS.Reservations.Domain.Reservations;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Data.Repository;

public class AzureSearchReservationIndexRepository(
    IAzureSearchHelper searchHelper,
    ILogger<AzureSearchReservationIndexRepository> logger)
    : IAzureSearchReservationIndexRepository
{
    public async Task<IndexedReservationSearchResult> Find(
        long providerId, string searchTerm, ushort pageNumber, ushort pageItemCount, SelectedSearchFilters selectedFilters)
    {
        logger.LogInformation("Starting reservation search");
        return await searchHelper.Find(providerId, searchTerm, pageNumber, pageItemCount, selectedFilters);

    }

    public async Task<HealthCheckResult> GetHealthCheckStatus(CancellationToken cancellationToken)
    {
        try
        {
            var indexName = await searchHelper.GetIndexName(cancellationToken);

            var indexCreatedDateTime = DateTime.ParseExact(
                indexName.Replace($"{AzureSearchIndex.IndexName}-", string.Empty),
                "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture);

            if (indexCreatedDateTime < DateTime.UtcNow.AddHours(-1))
            {
                return HealthCheckResult.Degraded;
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Unable to communicate with Azure search. Details: {details}", ex.Message);
            return HealthCheckResult.Degraded;
        }

        return HealthCheckResult.Healthy;
    }
}
