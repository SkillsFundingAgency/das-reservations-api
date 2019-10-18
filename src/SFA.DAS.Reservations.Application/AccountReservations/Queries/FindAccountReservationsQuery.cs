using MediatR;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class FindAccountReservationsQuery : IRequest<FindAccountReservationsResult>
    {
        public long AccountId { get; set; }
        public string SearchTerm { get; set; }
    }
}
