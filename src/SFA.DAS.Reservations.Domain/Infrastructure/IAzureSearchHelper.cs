using SFA.DAS.Reservations.Domain.Reservations;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Infrastructure;

public interface IAzureSearchHelper
{
    Task<string> GetIndexName(CancellationToken cancellationToken);
    Task<IndexedReservationSearchResult> Find(long providerId, string searchTerm, ushort pageNumber, ushort pageItemCount, SelectedSearchFilters selectedFilters);
}
