using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Application.Account.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IAccountRepository _repository;
        private readonly ReservationsConfiguration _configuration;

        public AccountsService (IAccountRepository repository, IOptions<ReservationsConfiguration> configuration)
        {
            _repository = repository;
            _configuration = configuration.Value;
        }
        public async Task<Domain.Account.Account> GetAccount(long accountId)
        {
            var account = await _repository.Get(accountId);

            return MapAccount(account);
        }

        private Domain.Account.Account MapAccount(Domain.Entities.Account account)
        {
            var reservationLimit = _configuration.MaxNumberOfReservations;

            if (account.ReservationLimit.HasValue)
            {
                reservationLimit = account.ReservationLimit.Value;
            }
            return new Domain.Account.Account
            {
                Id = account.Id,
                Name = account.Name,
                IsLevy = account.IsLevy,
                ReservationLimit = reservationLimit
            };
        }
    }
}