using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IAccountReservationService
    {
        Task<IList<Reservation>> GetAccountReservations(long accountId);
        Task<long> CreateAccountReservation(long accountId, DateTime startDate);
    }
}
