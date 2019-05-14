using MediatR;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries
{
    public class GetAccountLegalEntitiesQuery : IRequest<GetAccountLegalEntitiesResponse>
    {
        public long AccountId { get; set; }
    }
}