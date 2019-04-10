using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class GlobalRuleRepository
    {
        private readonly IReservationsDataContext _context;

        public GlobalRuleRepository(IReservationsDataContext context)
        {
            _context = context;
        }

        public async Task<IList<Domain.Entities.GlobalRule>> GetGlobalRules(DateTime dateFrom)
        {
            var globalRules = await _context.GlobalRules.Where(c => dateFrom >= c.ActiveFrom).ToListAsync();

            return globalRules;
        }
    }
}