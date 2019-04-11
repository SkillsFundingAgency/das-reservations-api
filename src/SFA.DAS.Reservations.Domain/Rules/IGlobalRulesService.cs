using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public interface IGlobalRulesService
    {
        Task<IList<GlobalRule>> GetRules();
    }
}