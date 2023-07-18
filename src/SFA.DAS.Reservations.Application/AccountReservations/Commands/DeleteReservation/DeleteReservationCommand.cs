using System;
using MediatR;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.DeleteReservation
{
    public class DeleteReservationCommand : IRequest
    {
        public Guid ReservationId { get; set; }
        public bool EmployerDeleted { get; set; }
    }
}