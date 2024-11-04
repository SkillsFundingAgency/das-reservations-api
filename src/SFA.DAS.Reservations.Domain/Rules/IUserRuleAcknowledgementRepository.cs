using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public interface IUserRuleAcknowledgementRepository
    {
        Task<Entities.UserRuleNotification> Add(Entities.UserRuleNotification userRuleNotification);
    }
}
