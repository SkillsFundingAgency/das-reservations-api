using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class GetAccountReservationsResult
    {
        public IList<Reservation> Reservations { get; set; }
    }
}