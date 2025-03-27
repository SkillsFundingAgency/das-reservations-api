using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using AccountLegalEntity = SFA.DAS.Reservations.Domain.Entities.AccountLegalEntity;
using SFA.DAS.Reservations.Domain.Exceptions;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class AccountLegalEntityRepository(IReservationsDataContext reservationsDataContext)
        : IAccountLegalEntitiesRepository
    {
        public async Task<IList<AccountLegalEntity>> GetByAccountId(long accountId)
        {
            var legalEntities = await reservationsDataContext
                .AccountLegalEntities
                .Where(c => c.AccountId.Equals(accountId))
                .ToListAsync();

            return legalEntities;
        }

        public async Task<AccountLegalEntity> Get(long accountLegalEntityId)
        {
            try
            {
                return await reservationsDataContext.AccountLegalEntities
                    .SingleAsync(entity => entity.AccountLegalEntityId == accountLegalEntityId);
            }
            catch (InvalidOperationException e)
            {
                throw new EntityNotFoundException<AccountLegalEntity>(e);
            }
        }
    }
}
