using System;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class IndexedReservationSearchResult
    {
        public SearchFilters Filters { get; set; } = new();
        public IEnumerable<ReservationIndex> Reservations { get; set; } = Array.Empty<ReservationIndex>();
        public uint TotalReservations { get; set; }
        public int TotalReservationsForProvider { get; set; }
    }
}
