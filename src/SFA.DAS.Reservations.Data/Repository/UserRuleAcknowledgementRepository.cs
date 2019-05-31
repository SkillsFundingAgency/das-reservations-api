using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Rules;
using UserRuleNotification = SFA.DAS.Reservations.Domain.Entities.UserRuleNotification;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class UserRuleAcknowledgementRepository : IUserRuleAcknowledgementRepository
    {
        private readonly IReservationsDataContext _reservationsDataContext;

        public UserRuleAcknowledgementRepository(IReservationsDataContext reservationsDataContext)
        {
            _reservationsDataContext = reservationsDataContext;
        }

        public async Task<UserRuleNotification> Add(UserRuleNotification userRuleNotification)
        {
            var result = await _reservationsDataContext.UserRuleNotifications.AddAsync(userRuleNotification);

            _reservationsDataContext.SaveChanges();

            return result.Entity;
        }
    }
}
