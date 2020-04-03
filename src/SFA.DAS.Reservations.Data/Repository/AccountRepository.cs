using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.Entities;
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

        public Task<Account> Get(long accountId)
        {
            try
            {
                return _reservationsDataContext.Accounts.FirstOrDefaultAsync(e => e.Id == accountId);
            }
            catch (InvalidOperationException e)
            {
                throw new EntityNotFoundException<AccountLegalEntity>(e);
            }
        }
    }
}