using System.Collections.Generic;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus;

namespace SFA.DAS.Reservations.Api.Models
{
    public class AccountReservationStatus
    {
        public AccountReservationStatus(GetAccountReservationStatusResponse response)
        {
            CanAutoCreateReservations = response.CanAutoCreateReservations;
            AccountLegalEntityAgreementStatus = response.AccountLegalEntityAgreementStatus;
            HasPendingReservations = response.HasPendingReservations;
            HasReachedReservationsLimit = response.HasReachedReservationsLimit;
        }

        public Dictionary<long, bool> AccountLegalEntityAgreementStatus { get ; }

        public bool CanAutoCreateReservations { get; }

        public bool HasReachedReservationsLimit { get; }
        public bool HasPendingReservations { get; }
    }
}