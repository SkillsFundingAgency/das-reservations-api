namespace SFA.DAS.Reservations.Domain.Rules
{
    public class GlobalRuleAccountExemption
    {

        public GlobalRuleAccountExemption(Entities.GlobalRuleAccountExemption accountExemption)
        {
            GlobalRuleId = accountExemption.GlobalRuleId;
            AccountId = accountExemption.AccountId;
        }

        public long GlobalRuleId{ get; set; }
        public long AccountId { get; set; }
    }
}
