using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Application.Account.Services
{
    public class AccountsService(IAccountRepository repository, IOptions<ReservationsConfiguration> configuration)
        : IAccountsService
    {
        private readonly ReservationsConfiguration _configuration = configuration.Value;

        public async Task<Domain.Account.Account> GetAccount(long accountId)
        {
            var account = await repository.Get(accountId);

            return MapAccount(account);
        }

        private Domain.Account.Account MapAccount(Domain.Entities.Account account)
        {
            return new Domain.Account.Account(account, _configuration.MaxNumberOfReservations);
        }
    }
}