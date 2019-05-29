using System;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class GlobalRule
    {
        public long Id { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
        public byte Restriction { get; set; }
        public byte RuleType { get; set; }
        public virtual UserRuleNotification UserRuleNotification { get; set; }
        public virtual IEnumerable<UserRuleNotification> UserRuleNotifications { get; set; }
    }
}