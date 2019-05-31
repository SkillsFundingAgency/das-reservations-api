using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public interface IUserRuleNotificationRepository
    {
        Task<Entities.UserRuleNotification> Add(Entities.UserRuleNotification userRuleNotification);
    }
}
