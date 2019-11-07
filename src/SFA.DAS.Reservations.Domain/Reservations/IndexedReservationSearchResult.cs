using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class IndexedReservationSearchResult
    {
        public IEnumerable<ReservationIndex> Reservations { get; set; }
        public long TotalReservations { get; set; }
    }
}
