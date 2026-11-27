using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationRepository
    {
        Task<IList<Entities.Reservation>> GetAccountReservations(long accountId);
        Task<int> CountPendingNonLevyReservations(long accountId);
        Task<int> CountReservationsTowardsLimit(long accountId, DateTime resetReservationDate);
        Task<Entities.Reservation> CreateAccountReservation(Entities.Reservation reservation);
        Task<Entities.Reservation> GetById(Guid id);
        Task DeleteAccountReservation(Guid reservationId);
        Task CreateAccountReservations(List<Entities.Reservation> reservations);
    }
}
