using System;
using MediatR;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.DeleteReservation
{
    public class DeleteReservationCommand : IRequest<Unit>
    {
        public Guid ReservationId { get; set; }
    }
}