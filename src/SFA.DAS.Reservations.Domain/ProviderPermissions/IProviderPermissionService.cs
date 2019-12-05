using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.ProviderPermissions
{
    public interface IProviderPermissionService
    {
        Task<IList<ProviderPermission>> GetPermissionsByProviderId(uint providerId);
    }
}