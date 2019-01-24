using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IAccountReservationService
    {
        Task<IList<Reservation>> GetAccountReservations(long accountId);
        Task<Reservation> CreateAccountReservation(Reservation reservation);
    }
}
