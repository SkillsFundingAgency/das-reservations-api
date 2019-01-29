using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Services
{
    public class RulesService : IRulesService
    {
        private readonly IRuleRepository _ruleRepository;

        public RulesService(IRuleRepository ruleRepository)
        {
            _ruleRepository = ruleRepository;
        }
        public async Task<IList<ReservationRule>> GetRules()
        {
            var repositoryRules = await _ruleRepository.GetReservationRules(DateTime.UtcNow);

            var rules = repositoryRules.Select(repositoryRule => new ReservationRule(repositoryRule)).ToList();

            return rules;
        }
    }
}
