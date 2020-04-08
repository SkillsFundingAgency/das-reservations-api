using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.Exceptions;
using Account = SFA.DAS.Reservations.Domain.Entities.Account;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IReservationsDataContext _reservationsDataContext;

        public AccountRepository(IReservationsDataContext reservationsDataContext)
        {
            _reservationsDataContext = reservationsDataContext;
        }

        public async Task<Account> Get(long accountId)
        {
            try
            {
                var account = await _reservationsDataContext.Accounts.FindAsync(accountId);
                return account;
            }
            catch (InvalidOperationException e)
            {
                throw new EntityNotFoundException<Account>(e);
            }
        }
    }
}