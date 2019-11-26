using System.Collections.Generic;

namespace SFA.DAS.Reservations.Application.ProviderPermissions.Queries
{
    public class GetAccountLegalEntitiesForProviderResponse
    {
        public IList<Domain.ProviderPermissions.ProviderPermission> ProviderPermissions { get ; set ; }
    }
}