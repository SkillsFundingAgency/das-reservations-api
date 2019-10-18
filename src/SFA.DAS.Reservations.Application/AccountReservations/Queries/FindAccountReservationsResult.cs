﻿using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class FindAccountReservationsResult
    {
        public ICollection<Reservation> Reservations { get; set; }
    }
}
