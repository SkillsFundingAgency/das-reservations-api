using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public class GlobalRule
    {
        public long Id { get; }
        public DateTime? ActiveFrom { get; }
        public DateTime? ActiveTo { get; }
        public GlobalRuleType RuleType { get; }
        public AccountRestriction Restriction { get; }
        public string RuleTypeText => Enum.GetName(typeof(GlobalRuleType), RuleType);
        public string RestrictionText => Enum.GetName(typeof(AccountRestriction), Restriction);
        public IEnumerable<UserRuleAcknowledgement> UserRuleAcknowledgements { get; }

        public GlobalRule(Entities.GlobalRule globalRule)
        {
            Id = globalRule.Id;
            ActiveFrom = globalRule.ActiveFrom;
            ActiveTo = globalRule.ActiveTo;
            RuleType = (GlobalRuleType)globalRule.RuleType;
            Restriction = (AccountRestriction) globalRule.Restriction;
            UserRuleAcknowledgements = globalRule.UserRuleNotifications?.Select(notification => new UserRuleAcknowledgement(notification));
        }
    }
}
