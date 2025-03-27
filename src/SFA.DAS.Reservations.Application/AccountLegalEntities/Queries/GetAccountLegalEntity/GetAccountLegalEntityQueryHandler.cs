using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntity
{
    public class GetAccountLegalEntityQueryHandler(
        IAccountLegalEntitiesService service,
        IValidator<GetAccountLegalEntityQuery> validator)
        : IRequestHandler<GetAccountLegalEntityQuery, GetAccountLegalEntityResult>
    {
        public async Task<GetAccountLegalEntityResult> Handle(GetAccountLegalEntityQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation", validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var accountLegalEntity = await service.GetAccountLegalEntity(request.Id);

            return new GetAccountLegalEntityResult { LegalEntity= accountLegalEntity };
        }
    }
}
