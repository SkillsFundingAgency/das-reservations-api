using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Services
{
    public class GlobalRulesService : IGlobalRulesService
    {
        private readonly IGlobalRuleRepository _repository;

        public GlobalRulesService(IGlobalRuleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<GlobalRule>> GetRules()
        {
            var result = await _repository.GetGlobalRules(DateTime.UtcNow);

            var globalRules = result.Select(globalRule => new GlobalRule(globalRule)).ToList();
            return globalRules;
        }
    }
}