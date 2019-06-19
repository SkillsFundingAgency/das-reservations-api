using System;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations
{
    public class BulkCreateAccountReservationsResult
    {
        public IEnumerable<Guid> CreatedReservationIds { get; set; }
    }
}
