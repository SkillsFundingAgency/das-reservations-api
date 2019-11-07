using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationIndexRepository
    {
        Task<IndexedReservationSearchResult> Find(long providerId, string term, ushort pageNumber, ushort pageItemCount);
    }
}
