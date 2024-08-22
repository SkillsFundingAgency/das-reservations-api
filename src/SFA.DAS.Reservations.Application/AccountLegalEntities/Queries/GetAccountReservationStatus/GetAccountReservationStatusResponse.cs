using System.Collections.Generic;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus
{
    public class GetAccountReservationStatusResponse
    {
        public bool CanAutoCreateReservations { get; set; }
        public bool HasReachedReservationsLimit { get; set; }
        public bool HasPendingReservations { get; set; }
        public Dictionary<long,bool> AccountLegalEntityAgreementStatus { get ; set ; }
    }
}