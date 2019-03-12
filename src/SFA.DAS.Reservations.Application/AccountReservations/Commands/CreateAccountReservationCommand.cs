using System;
using MediatR;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands
{
    public class CreateAccountReservationCommand : IRequest<CreateAccountReservationResult>
    {
        public Guid Id { get; set; }
        public long AccountId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
