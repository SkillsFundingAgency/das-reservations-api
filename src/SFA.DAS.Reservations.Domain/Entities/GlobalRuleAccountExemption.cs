namespace SFA.DAS.Reservations.Domain.Entities
{
    public class GlobalRuleAccountExemption
    {
        public long GlobalRuleId { get; set; }
        public long AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual GlobalRule GlobalRule { get; set; }
    }
}
