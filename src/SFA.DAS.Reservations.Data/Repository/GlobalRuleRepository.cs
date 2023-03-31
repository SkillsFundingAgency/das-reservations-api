﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.Rules;
using GlobalRule = SFA.DAS.Reservations.Domain.Entities.GlobalRule;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class GlobalRuleRepository : IGlobalRuleRepository
    {
        private readonly IReservationsDataContext _context;

        public GlobalRuleRepository(IReservationsDataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<GlobalRule>> GetAll()
        {
            return await _context.GlobalRules.ToListAsync();
        }

        public async Task<ICollection<GlobalRule>> FindActive(DateTime dateFrom)
        {
            return await _context.GlobalRules
                .Include(x => x.GlobalRuleAccountExemptions)
                .Where(c => dateFrom >= c.ActiveFrom && (c.ActiveTo == null || dateFrom < c.ActiveTo)).ToListAsync();
        }
    }
}