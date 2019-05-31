using System.Threading.Tasks;
using SFA.DAS.Reservations.Application.Rules.Commands;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Services
{
    public class UserRuleNotificationService
    {
        private readonly IUserRuleNotificationRepository _userRuleNotificationRepository;

        public UserRuleNotificationService(IUserRuleNotificationRepository userRuleNotificationRepository)
        {
            _userRuleNotificationRepository = userRuleNotificationRepository;
        }

        public async Task<UserRuleNotification> CreateUserRuleNotification(CreateUserRuleNotification createUserRuleNotification)
        {
            var result = await _userRuleNotificationRepository.Add(MapUserRuleNotification(createUserRuleNotification));

            return MapUserRuleNotification(result);
        }

        private Domain.Entities.UserRuleNotification MapUserRuleNotification(CreateUserRuleNotification createUserRuleNotification)
        {
            var userRuleNotification = new UserRuleNotification(createUserRuleNotification.Id,createUserRuleNotification.RuleId, createUserRuleNotification.RuleType);

            return new Domain.Entities.UserRuleNotification
            {
                CourseRuleId = userRuleNotification.CourseRuleId,
                GlobalRuleId = userRuleNotification.GlobalRuleId,
                UkPrn = userRuleNotification.UkPrn,
                UserId = userRuleNotification.UserId
            };
        }

        private UserRuleNotification MapUserRuleNotification(Domain.Entities.UserRuleNotification entity)
        {
            return new UserRuleNotification(entity);
        }
    }
}
