using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateReservationsWithNonLevy;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Api.Models
{
    public class BulkCreateReservationsWithNonLevyRequest
    {
        public List<BulkCreateReservations> Reservations { get; set; }
    }
}
