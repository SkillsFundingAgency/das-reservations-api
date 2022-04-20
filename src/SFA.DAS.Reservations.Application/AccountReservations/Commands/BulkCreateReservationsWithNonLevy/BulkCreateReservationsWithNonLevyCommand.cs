using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateReservationsWithNonLevy
{
    public class BulkCreateReservationsWithNonLevyCommand : IRequest<BulkCreateReservationsWithNonLevyResult>
    {
        public List<BulkCreateReservations> Reservations { get; set; }
    }
}
