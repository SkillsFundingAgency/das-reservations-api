using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class IndexedReservationSearchResult
    {
        public IndexedReservationSearchResult()
        {
            Reservations = new ReservationIndex[0];
            Filters = new SearchFilters();
        }

        public SearchFilters Filters { get; set; }
        public IEnumerable<ReservationIndex> Reservations { get; set; }
        public uint TotalReservations { get; set; }
    }
}
