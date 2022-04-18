using MediatR;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateReservationsWithNoneLevy
{
    public class BulkCreateReservationsWithNoneLevyCommand : IRequest<BulkCreateReservationsWithNoneLevyResult>
    {
        public List<BulkCreateReservations> Reservations { get; set; }
    }
}
