using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Services
{
    public class UserRuleAcknowledgementService(IUserRuleAcknowledgementRepository userRuleAcknowledgementRepository)
        : IUserRuleAcknowledgementService
    {
        public async Task<UserRuleAcknowledgement> CreateUserRuleAcknowledgement(IUserRuleAcknowledgementRequest request)
        {
            var entity = MapUserRuleAcknowledgement(request);

            var result = await userRuleAcknowledgementRepository.Add(entity);

            return MapUserRuleAcknowledgement(result);
        }

        private static Domain.Entities.UserRuleNotification MapUserRuleAcknowledgement(IUserRuleAcknowledgementRequest request)
        {
            var userRuleNotification = new UserRuleAcknowledgement(request.Id, request.RuleId, request.TypeOfRule);

            return new Domain.Entities.UserRuleNotification
            {
                CourseRuleId = userRuleNotification.CourseRuleId,
                GlobalRuleId = userRuleNotification.GlobalRuleId,
                UkPrn = userRuleNotification.UkPrn,
                UserId = userRuleNotification.UserId
            };
        }

        private static UserRuleAcknowledgement MapUserRuleAcknowledgement(Domain.Entities.UserRuleNotification entity)
        {
            return new UserRuleAcknowledgement(entity);
        }
    }
}
