using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class Account
    {
        public long Id { get; set; }
        public string Name { get; set; } 
        public bool IsLevy { get; set; }
        public virtual ICollection<ProviderPermission> ProviderPermissions { get; set; }
        public virtual ICollection<AccountLegalEntity> AccountLegalEntities { get; set; }
    }
}