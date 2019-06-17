using System;

namespace SFA.DAS.Reservations.Domain.AccountLegalEntities
{
    public class AccountLegalEntity
    {
        public Guid Id { get; }
        public long AccountId { get; }
        public string AccountLegalEntityName { get; }
        public long LegalEntityId { get; }
        public long AccountLegalEntityId { get; }
        public int ReservationLimit { get; }
        public bool AgreementSigned { get; }
        public bool IsLevy { get; }

        public AccountLegalEntity(
            Guid id, 
            long accountId,
            string accountLegalEntityName, 
            long legalEntityId,
            long accountLegalEntityId, 
            int reservationLimit, 
            bool agreementSigned,
            bool isLevy)
        {
            Id = id;
            AccountId = accountId;
            AccountLegalEntityName = accountLegalEntityName;
            LegalEntityId = legalEntityId;
            AccountLegalEntityId = accountLegalEntityId;
            ReservationLimit = reservationLimit;
            AgreementSigned = agreementSigned;
            IsLevy = isLevy;
        }
    }
}
