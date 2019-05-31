using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Commands
{
    public class CreateUserRuleNotification : IUserRuleNotificationRequest
    {
        public string Id { get; set; }
        public long RuleId { get; set; }
        public RuleType RuleType { get; set; }
    }
}
