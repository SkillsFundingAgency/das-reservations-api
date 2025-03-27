using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.ProviderPermissions.Queries
{
    public class GetAccountLegalEntitiesForProviderQueryHandler(
        IProviderPermissionRepository service,
        IValidator<GetAccountLegalEntitiesForProviderQuery> validator)
        : IRequestHandler<GetAccountLegalEntitiesForProviderQuery, GetAccountLegalEntitiesForProviderResponse>
    {
        public async Task<GetAccountLegalEntitiesForProviderResponse> Handle(GetAccountLegalEntitiesForProviderQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation", validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var permissions = await service.GetAllowedNonLevyPermissionsForProvider(request.ProviderId);
            
            return new GetAccountLegalEntitiesForProviderResponse
            {
                ProviderPermissions = permissions
            }; 
        }
    }
}