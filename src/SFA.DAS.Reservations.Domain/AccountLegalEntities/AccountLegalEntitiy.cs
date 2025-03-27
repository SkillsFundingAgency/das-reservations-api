using System;

namespace SFA.DAS.Reservations.Domain.AccountLegalEntities
{
    public class AccountLegalEntity(
        Guid id,
        long accountId,
        string accountLegalEntityName,
        long legalEntityId,
        long accountLegalEntityId,
        bool agreementSigned,
        bool isLevy)
    {
        public Guid Id { get; } = id;
        public long AccountId { get; } = accountId;
        public string AccountLegalEntityName { get; } = accountLegalEntityName;
        public long LegalEntityId { get; } = legalEntityId;
        public long AccountLegalEntityId { get; } = accountLegalEntityId;
        public bool AgreementSigned { get; } = agreementSigned;
        public bool IsLevy { get; } = isLevy;
    }
}
