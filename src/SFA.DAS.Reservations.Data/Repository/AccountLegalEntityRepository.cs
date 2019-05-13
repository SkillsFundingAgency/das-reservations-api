using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class AccountLegalEntityRepository : IAccountLegalEntitiesRepository
    {
        private readonly IReservationsDataContext _reservationsDataContext;

        public AccountLegalEntityRepository(IReservationsDataContext reservationsDataContext)
        {
            _reservationsDataContext = reservationsDataContext;
        }

        public async Task<IList<Domain.Entities.AccountLegalEntity>> GetByAccountId(long accountId)
        {
            var legalEntities = await _reservationsDataContext.AccountLegalEntities.Where(c => c.AccountId.Equals(accountId)).ToListAsync();

            return legalEntities;

        }
    }
}
