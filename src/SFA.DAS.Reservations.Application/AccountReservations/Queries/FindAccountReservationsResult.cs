using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class FindAccountReservationsResult
    {
        public IEnumerable<Reservation> Reservations { get; set; }
        public uint NumberOfRecordsFound { get; set; }
        public SearchFilters Filters { get; set; }
        public int TotalReservationsForProvider { get; set; }
    }
}
