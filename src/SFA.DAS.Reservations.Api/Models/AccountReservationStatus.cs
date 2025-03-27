using System.Collections.Generic;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus;

namespace SFA.DAS.Reservations.Api.Models
{
    public class AccountReservationStatus(GetAccountReservationStatusResponse response)
    {
        public Dictionary<long, bool> AccountLegalEntityAgreementStatus { get ; } = response.AccountLegalEntityAgreementStatus;

        public bool CanAutoCreateReservations { get; } = response.CanAutoCreateReservations;

        public bool HasReachedReservationsLimit { get; } = response.HasReachedReservationsLimit;
        public bool HasPendingReservations { get; } = response.HasPendingReservations;
    }
}