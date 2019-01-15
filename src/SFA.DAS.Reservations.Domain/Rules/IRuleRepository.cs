using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public interface IRuleRepository
    {
        Task<IList<Entities.Rule>> GetReservationRules(DateTime startDate, DateTime? endDate);
    }
}
