using System;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public class UserRuleAcknowledgement : IUserRuleAcknowledgementRequest
    {
        public UserRuleAcknowledgement(string id, long ruleId, RuleType ruleType)
        {
            switch (ruleType)
            {
                case RuleType.GlobalRule:
                    GlobalRuleId = ruleId;
                    break;
                case RuleType.CourseRule:
                    CourseRuleId = ruleId;
                    break;
            }

            if (int.TryParse(id, out var ukPrnResult))
            {
                UkPrn = ukPrnResult;
            }
            else if (Guid.TryParse(id, out var userIdResult))
            {
                UserId = userIdResult;
            }
        }

        public UserRuleAcknowledgement(Entities.UserRuleNotification entity)
        {
            UserRuleNotificationId = entity.Id;
            CourseRuleId = entity.CourseRuleId;
            GlobalRuleId = entity.GlobalRuleId;
            UkPrn = entity.UkPrn;
            UserId = entity.UserId;
        }

        public long UserRuleNotificationId { get; }

        public string Id { get; set; }
        public long RuleId { get; set; }
        public RuleType RuleType { get; set; }

        public Guid UserId { get; }
        public int UkPrn { get; }
        public long CourseRuleId { get; }
        public long GlobalRuleId { get; }
    }
}
