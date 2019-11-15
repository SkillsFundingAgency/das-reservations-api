using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationIndexRepository
    {
        Task<IndexedReservationSearchResult> Find(
            long providerId, string searchTerm, ushort pageNumber, 
            ushort pageItemCount, SearchFilters selectedFilters);
    }
}
