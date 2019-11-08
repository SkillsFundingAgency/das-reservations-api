using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class IndexedReservationSearchResult
    {
        public IndexedReservationSearchResult()
        {
            Reservations = new ReservationIndex[0];
        }

        public IEnumerable<ReservationIndex> Reservations { get; set; }
        public uint TotalReservations { get; set; }
    }
}
