using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Services
{
    public class RulesService(IRuleRepository ruleRepository) : IRulesService
    {
        public async Task<IList<ReservationRule>> GetRules()
        {
            var repositoryRules = await ruleRepository.GetReservationRules(DateTime.UtcNow);

            var rules = repositoryRules.Select(repositoryRule => new ReservationRule(repositoryRule)).ToList();

            return rules;
        }
    }
}
