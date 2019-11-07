using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class ReservationSearchResult
    {
        public IEnumerable<Reservation> Reservations { get; set; }
        public long TotalReservations { get; set; }
    }
}
