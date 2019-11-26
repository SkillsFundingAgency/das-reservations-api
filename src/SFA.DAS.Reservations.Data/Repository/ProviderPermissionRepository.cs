using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ProviderPermissionRepository
    {
        private readonly IReservationsDataContext _dataContext;

        public ProviderPermissionRepository(IReservationsDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IList<ProviderPermission>> GetByProviderId(uint ukPrn)
        {
            var providerPermissions =
                await _dataContext.ProviderPermissions.Where(c => c.UkPrn.Equals(ukPrn)).ToListAsync();

            return providerPermissions;
        }
    }
}