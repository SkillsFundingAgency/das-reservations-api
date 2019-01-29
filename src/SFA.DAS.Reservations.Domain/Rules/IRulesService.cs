using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public interface IRulesService
    {
        Task<IList<ReservationRule>> GetRules();
    }
}
