using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IAccountReservationService
    {
        Task<IList<Reservation>> GetAccountReservations(long accountId);
        Task<Reservation> GetReservation(Guid id);
        Task<ReservationSearchResult> FindReservations(
            long accountId, string searchTerm, ushort pageNumber, ushort pageItemCount, SelectedSearchFilters selectedFilters);
        Task<Reservation> CreateAccountReservation(IReservationRequest command);
        Task DeleteReservation(Guid reservationId);
        Task<IList<Guid>> BulkCreateAccountReservation(uint reservationCount, long accountLegalEntityId, long accountId, string accountLegalEntityName, long? transferSenderAccountId);
        Task<Guid> ChangeOfParty(ChangeOfPartyServiceRequest request);
        Task<int> GetRemainingReservations(long accountId, int totalReservationAllowed);
    }
}
