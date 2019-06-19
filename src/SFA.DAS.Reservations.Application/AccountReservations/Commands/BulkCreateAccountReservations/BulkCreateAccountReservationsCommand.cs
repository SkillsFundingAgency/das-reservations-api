using MediatR;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations
{
    public class BulkCreateAccountReservationsCommand : IRequest<BulkCreateAccountReservationsResult>
    {
        public uint ReservationCount { get; set; }
    }
}
