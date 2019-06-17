using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public interface IUserRuleAcknowledgementService
    {
        Task<UserRuleAcknowledgement> CreateUserRuleAcknowledgement(IUserRuleAcknowledgementRequest request);
    }
}
