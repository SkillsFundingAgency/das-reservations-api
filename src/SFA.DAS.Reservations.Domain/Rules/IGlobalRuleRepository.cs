using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public interface IGlobalRuleRepository
    {
        Task<IList<Domain.Entities.GlobalRule>> GetGlobalRules(DateTime dateFrom);
    }
}