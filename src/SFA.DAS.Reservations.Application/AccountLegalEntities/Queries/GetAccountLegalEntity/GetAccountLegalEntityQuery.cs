using MediatR;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntity
{
    public class GetAccountLegalEntityQuery : IRequest<GetAccountLegalEntityResult>
    {
        public long Id { get; set; }
    }
}
