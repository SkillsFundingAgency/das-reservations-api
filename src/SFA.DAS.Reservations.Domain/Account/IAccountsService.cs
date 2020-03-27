using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Account
{
    public interface IAccountsService
    {
        Task<Domain.Account.Account> GetAccount(long accountId);
    }
}