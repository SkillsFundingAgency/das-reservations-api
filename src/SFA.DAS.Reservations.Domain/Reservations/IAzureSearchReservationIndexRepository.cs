using SFA.DAS.Reservations.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations;

public interface IAzureSearchReservationIndexRepository
{
    Task<IndexedReservationSearchResult> Find(
        long providerId, string searchTerm, ushort pageNumber,
        ushort pageItemCount, SelectedSearchFilters selectedFilters);

    Task<HealthCheckResult> GetHealthCheckStatus(CancellationToken cancellationToken);
}
