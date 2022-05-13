using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateReservationsWithNonLevy
{
    public class BulkCreateReservationsCommand : IRequest<BulkCreateReservationsWithNonLevyResult>
    {
        public List<BulkCreateReservation> Reservations { get; set; }
    }
}
