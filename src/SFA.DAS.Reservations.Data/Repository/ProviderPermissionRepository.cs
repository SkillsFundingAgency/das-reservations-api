using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ProviderPermissionRepository : IProviderPermissionRepository
    {
        private readonly IReservationsDataContext _dataContext;

        public ProviderPermissionRepository(IReservationsDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IList<Domain.Entities.ProviderPermission>> GetByProviderId(uint ukPrn)
        {
            var providerPermissions =
                await _dataContext.ProviderPermissions.Where(c => c.UkPrn.Equals(ukPrn)).ToListAsync();

            return providerPermissions;
        }
    }
}