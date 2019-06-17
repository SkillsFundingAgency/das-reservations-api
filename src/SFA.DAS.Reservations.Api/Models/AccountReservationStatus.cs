using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus;

namespace SFA.DAS.Reservations.Api.Models
{
    public class AccountReservationStatus
    {
        public AccountReservationStatus(GetAccountReservationStatusResponse response)
        {
            CanAutoCreateReservations = response.CanAutoCreateReservations;
        }

        public bool CanAutoCreateReservations { get; }
    }
}