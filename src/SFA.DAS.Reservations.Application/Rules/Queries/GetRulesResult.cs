using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetRulesResult
    {
        public IList<ReservationRule> Rules { get; set; }
        public IList<GlobalRule> GlobalRule { get; set; }
    }
}