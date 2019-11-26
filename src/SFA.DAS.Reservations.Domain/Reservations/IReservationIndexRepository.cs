using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationIndexRepository
    {
        Task<bool> PingAsync();
        Task<IndexRegistryEntry> GetCurrentReservationIndex();
        Task<IndexedReservationSearchResult> Find(
            long providerId, string searchTerm, ushort pageNumber, 
            ushort pageItemCount, SelectedSearchFilters selectedFilters);
    }
}
