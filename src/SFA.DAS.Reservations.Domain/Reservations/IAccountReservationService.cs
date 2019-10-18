using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IAccountReservationService
    {
        Task<IList<Reservation>> GetAccountReservations(long accountId);
        Task<Reservation> GetReservation(Guid id);
        Task<IList<Reservation>> FindReservations(long accountId, string searchTerm);
        Task<Reservation> CreateAccountReservation(IReservationRequest command);
        Task DeleteReservation(Guid reservationId);
        Task<IList<Guid>> BulkCreateAccountReservation(uint reservationCount, long accountLegalEntityId, long accountId, string accountLegalEntityName, long? transferSenderAccountId);
    }
}
