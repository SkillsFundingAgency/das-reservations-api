using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesResponse
    {
        public IList<AccountLegalEntity> AccountLegalEntities { get; set; }
    }
}