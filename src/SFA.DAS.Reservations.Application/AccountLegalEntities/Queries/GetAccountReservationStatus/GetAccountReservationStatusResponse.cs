using System.Collections.Generic;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus
{
    public class GetAccountReservationStatusResponse
    {
        public bool CanAutoCreateReservations { get; set; }
        public Dictionary<long,bool> AccountLegalEntityAgreementStatus { get ; set ; }
    }
}