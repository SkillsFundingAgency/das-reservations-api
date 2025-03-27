using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesQueryHandler(
        IValidator<GetAccountLegalEntitiesQuery> validator,
        IAccountLegalEntitiesService service)
        : IRequestHandler<GetAccountLegalEntitiesQuery, GetAccountLegalEntitiesResponse>
    {
        public async Task<GetAccountLegalEntitiesResponse> Handle(GetAccountLegalEntitiesQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation", validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var accountLegalEntities = await service.GetAccountLegalEntities(request.AccountId);

            return new GetAccountLegalEntitiesResponse { AccountLegalEntities= accountLegalEntities };
        }
    }
}
