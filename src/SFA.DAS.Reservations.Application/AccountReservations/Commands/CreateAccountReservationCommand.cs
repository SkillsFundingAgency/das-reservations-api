using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands
{
    public class CreateAccountReservationCommand : IRequest<CreateAccountReservationResult>
    {
        public Reservation Reservation { get; set; }
    }
}
