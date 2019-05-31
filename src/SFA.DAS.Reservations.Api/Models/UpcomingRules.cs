using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Api.Models
{
    public class UpcomingRules
    {
        public string Id { get; set; }
        public long RuleId { get; set; }
        public RuleType TypeOfRule {get;set;}
    }
}
