namespace SFA.DAS.Reservations.Domain.Rules
{
    public class GlobalRuleAccountExemption(Entities.GlobalRuleAccountExemption accountExemption)
    {
        public long GlobalRuleId{ get; set; } = accountExemption.GlobalRuleId;
        public long AccountId { get; set; } = accountExemption.AccountId;
    }
}
