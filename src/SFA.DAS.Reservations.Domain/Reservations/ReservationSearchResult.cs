﻿using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class ReservationSearchResult
    {
        public IEnumerable<Reservation> Reservations { get; set; }
        public uint TotalReservations { get; set; }
    }
}
