using System;
using System.Collections.Generic;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class AccountLegalEntity
    {
        public Guid Id { get; set; }
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string AccountLegalEntityName { get; set; }
        public int? ReservationLimit { get; set; }
        public bool AgreementSigned { get; set; }
        public virtual Account Account { get; set; }
        public virtual ICollection<ProviderPermission> ProviderPermissions { get; set; }
    }
}
