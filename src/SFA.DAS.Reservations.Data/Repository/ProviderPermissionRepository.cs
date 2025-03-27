using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.ProviderPermissions;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ProviderPermissionRepository(IReservationsDataContext dataContext) : IProviderPermissionRepository
    {
        public async Task<IList<ProviderPermission>> GetAllowedNonLevyPermissionsForProvider(uint ukPrn)
        {
            var providerPermissions =
                await dataContext
                .ProviderPermissions
                .Include(p => p.AccountLegalEntity)
                .Where(pp =>
                    pp.UkPrn.Equals(ukPrn)
                    && pp.CanCreateCohort
                    && pp.AccountLegalEntity != null
                    && pp.Account != null
                    && !pp.Account.IsLevy)
                .Select(providerPermission => new ProviderPermission
                {
                    AccountId = providerPermission.AccountId,
                    AgreementSigned = providerPermission.AccountLegalEntity.AgreementSigned,
                    UkPrn = (uint)providerPermission.UkPrn,
                    AccountLegalEntityId = providerPermission.AccountLegalEntityId,
                    AccountLegalEntityName = providerPermission.AccountLegalEntity.AccountLegalEntityName,
                    AccountName = providerPermission.Account.Name
                })
                .ToListAsync();

            return providerPermissions;
        }
    }
}