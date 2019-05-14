using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.AccountLegalEntities
{
    public interface IAccountLegalEntitiesService
    {
        Task<IList<AccountLegalEntity>> GetAccountLegalEntities(long accountId);
    }
}