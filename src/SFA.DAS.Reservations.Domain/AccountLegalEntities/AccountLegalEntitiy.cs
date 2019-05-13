using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Reservations.Domain.AccountLegalEntities
{
    public class AccountLegalEntity
    {
        public Guid Id { get; }
        public long AccountId { get; }
        public string AccountLegalEntityName { get; }
        public long LegalEntityId { get; }
        public long AccountLegalEntityId { get; }

        public AccountLegalEntity(Guid id, long accountId, string accountLegalEntityName, long legalEntityId, long accountLegalEntityId)
        {
            Id = id;
            AccountId = accountId;
            AccountLegalEntityName = accountLegalEntityName;
            LegalEntityId = legalEntityId;
            AccountLegalEntityId = accountLegalEntityId;
        }
    }
}
