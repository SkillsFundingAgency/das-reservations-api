using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.Exceptions;
using Account = SFA.DAS.Reservations.Domain.Entities.Account;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class AccountRepository(IReservationsDataContext reservationsDataContext) : IAccountRepository
    {
        public async Task<Account> Get(long accountId)
        {
            try
            {
                var account = await reservationsDataContext.Accounts.FindAsync(accountId);
                return account;
            }
            catch (InvalidOperationException e)
            {
                throw new EntityNotFoundException<Account>(e);
            }
        }
    }
}