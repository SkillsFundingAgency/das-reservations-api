using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationRepository
    {
        Task<IList<Entities.Reservation>> GetAccountReservations(long accountId);
        Task<long> CreateAccountReservation(Entities.Reservation reservation);
    }
}
