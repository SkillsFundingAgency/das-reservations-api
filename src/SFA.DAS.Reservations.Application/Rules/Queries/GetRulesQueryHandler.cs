using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetRulesQueryHandler : IRequestHandler<GetRulesQuery, GetRulesResult>
    {
        private readonly IRulesService _rulesService;
        private readonly IGlobalRulesService _globalRulesService;

        public GetRulesQueryHandler(IRulesService rulesService, IGlobalRulesService globalRulesService)
        {
            _rulesService = rulesService;
            _globalRulesService = globalRulesService;
        }

        public async Task<GetRulesResult> Handle(GetRulesQuery request, CancellationToken cancellationToken)
        {
            var globalRules = await _globalRulesService.GetAllRules();

            if (globalRules.Any())
            {
                return new GetRulesResult
                {
                    GlobalRules = globalRules
                };
            }

            var rules = await _rulesService.GetRules();

            return new GetRulesResult
            {
                Rules = rules
            };
        }
    }
}
