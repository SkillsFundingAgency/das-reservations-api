using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public interface IUserRuleNotificationRequest
    {
        string Id { get; set; }
        long RuleId { get; set; }
        RuleType RuleType { get; set; }
    }
}
