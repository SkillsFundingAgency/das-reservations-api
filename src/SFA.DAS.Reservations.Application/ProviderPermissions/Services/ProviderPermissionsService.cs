using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Application.ProviderPermissions.Services
{
    public class ProviderPermissionsService : IProviderPermissionService
    {
        private readonly IProviderPermissionRepository _permissionRepository;

        public ProviderPermissionsService (IProviderPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }
        public async Task<IList<ProviderPermission>> GetPermissionsByProviderId(uint providerId)
        {
            var result = await _permissionRepository.GetByProviderId(providerId);

            return result.Where(x => 
                    x.CanCreateCohort 
                    && x.AccountLegalEntity != null 
                    && x.Account != null
                    && !x.AccountLegalEntity.IsLevy)
                .Select(c => new ProviderPermission(c)).ToList();
        }
    }
}