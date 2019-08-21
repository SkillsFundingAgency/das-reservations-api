using MediatR;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus
{
    public class GetAccountReservationStatusQuery : IRequest<GetAccountReservationStatusResponse>
    {
        public long AccountId { get; set; }
        public long? TransferSenderAccountId { get; set; }
    }
}