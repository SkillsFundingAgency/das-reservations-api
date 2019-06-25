using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation
{
    public class CreateAccountReservationResult
    {
        public Reservation Reservation { get; set; }
        public GlobalRule Rule { get; set; }
    }
}