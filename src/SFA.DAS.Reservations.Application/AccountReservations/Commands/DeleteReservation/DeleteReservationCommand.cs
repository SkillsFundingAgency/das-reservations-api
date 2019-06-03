using System;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.DeleteReservation
{
    public class DeleteReservationCommand
    {
        public Guid ReservationId { get; set; }
    }
}