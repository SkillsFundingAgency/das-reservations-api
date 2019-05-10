using MediatR;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetAccountRulesQuery : IRequest<GetAccountRulesResult>
    {
        public long AccountId { get; set; }
    }
}