using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class FindAccountReservationsQuery : IRequest<FindAccountReservationsResult>
    {
        public long ProviderId { get; set; }
        public string SearchTerm { get; set; }
        public ushort PageNumber { get; set; }
        public ushort PageItemCount { get; set; }
        public SelectedSearchFilters SelectedFilters { get; set; }
    }
}
