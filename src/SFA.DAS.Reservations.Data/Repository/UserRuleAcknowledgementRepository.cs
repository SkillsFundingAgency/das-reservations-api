using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Rules;
using UserRuleNotification = SFA.DAS.Reservations.Domain.Entities.UserRuleNotification;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class UserRuleAcknowledgementRepository(IReservationsDataContext reservationsDataContext)
        : IUserRuleAcknowledgementRepository
    {
        public async Task<UserRuleNotification> Add(UserRuleNotification userRuleNotification)
        {
            var result = await reservationsDataContext.UserRuleNotifications.AddAsync(userRuleNotification);

            reservationsDataContext.SaveChanges();

            return result.Entity;
        }
    }
}
