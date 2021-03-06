﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class RuleRepository : IRuleRepository
    {
        private readonly IReservationsDataContext _reservationsDataContext;

        public RuleRepository(IReservationsDataContext reservationsDataContext)
        {
            _reservationsDataContext = reservationsDataContext;
        }
        public async Task<IList<Domain.Entities.Rule>> GetReservationRules(DateTime startDate)
        {
            
            var endDate = DateTime.UtcNow;
            
            var result = await _reservationsDataContext
                .Rules
                .Where(c => startDate >= c.ActiveFrom && c.ActiveTo >= endDate)
                .Include(c=>c.Course)
                .ToListAsync();

            return result;
        }
    }
}