using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetGlobalRulesResult
    {
        public IList<GlobalRule> GlobalRules { get; set; }
    }
}