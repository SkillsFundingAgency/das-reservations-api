using MediatR;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class GetAccountReservationsQuery : IRequest<GetAccountReservationsResult>
    {
        public long AccountId { get; set; }
    }
}