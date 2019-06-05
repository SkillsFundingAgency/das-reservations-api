﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.AccountLegalEntities
{
    public interface IAccountLegalEntitiesRepository
    {
        Task<IList<Entities.AccountLegalEntity>> GetByAccountIdWithAgreementSigned(long accountId);
        Task<Entities.AccountLegalEntity> Get(long accountLegalEntityId);
    }
}
