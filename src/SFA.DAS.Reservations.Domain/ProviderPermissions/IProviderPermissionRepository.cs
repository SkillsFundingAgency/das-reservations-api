using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.ProviderPermissions
{
    public interface IProviderPermissionRepository
    {
        Task<IList<Domain.Entities.ProviderPermission>> GetByProviderId(uint ukPrn);
    }
}