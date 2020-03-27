using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Account
{
    public interface IAccountRepository
    {
        Task<Domain.Entities.Account> Get(long accountId);
    }    
}