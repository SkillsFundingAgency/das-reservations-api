using MediatR;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesQuery : IRequest<GetAccountLegalEntitiesResponse>
    {
        public long AccountId { get; set; }
    }
}