namespace SFA.DAS.Reservations.Domain.Rules
{
    public interface IUserRuleAcknowledgementRequest
    {
        string Id { get; set; }
        long RuleId { get; set; }
        RuleType TypeOfRule { get; set; }
    }
}
