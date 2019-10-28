using MediatR;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class FindAccountReservationsQuery : IRequest<FindAccountReservationsResult>
    {
        public long ProviderId { get; set; }
        public string SearchTerm { get; set; }
    }
}
