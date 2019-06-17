using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IAccountReservationService
    {
        Task<IList<Reservation>> GetAccountReservations(long accountId);
        Task<Reservation> GetReservation(Guid id);
        Task<Reservation> CreateAccountReservation(IReservationRequest command);
        Task DeleteReservation(Guid reservationId);
    }
}
