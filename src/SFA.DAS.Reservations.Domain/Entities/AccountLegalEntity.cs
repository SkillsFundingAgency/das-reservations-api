using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
