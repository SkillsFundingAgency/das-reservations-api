using System;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public class GlobalRule
    {
        public GlobalRule(Entities.GlobalRule globalRule)
        {
            Id = globalRule.Id;
            ActiveFrom = globalRule.ActiveFrom;
            RuleType = (GlobalRuleType)globalRule.RuleType;
            Restriction = (AccountRestriction) globalRule.Restriction;
        }

        public long Id { get; }
        public DateTime? ActiveFrom { get; }
        public GlobalRuleType RuleType { get; }
        public AccountRestriction Restriction { get; }
    }
}
