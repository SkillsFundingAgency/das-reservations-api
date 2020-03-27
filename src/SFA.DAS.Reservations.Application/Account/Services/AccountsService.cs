using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Account;

namespace SFA.DAS.Reservations.Application.Account.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IAccountRepository _repository;

        public AccountsService (IAccountRepository repository)
        {
            _repository = repository;
        }
        public async Task<Domain.Account.Account> GetAccount(long accountId)
        {
            var account = await _repository.Get(accountId);

            return MapAccount(account);
        }

        private Domain.Account.Account MapAccount(Domain.Entities.Account account)
        {
            return new Domain.Account.Account
            {
                Id = account.Id,
                Name = account.Name,
                IsLevy = account.IsLevy
            };
        }
    }
}