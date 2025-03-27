using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetRulesQueryHandler(IRulesService rulesService, IGlobalRulesService globalRulesService)
        : IRequestHandler<GetRulesQuery, GetRulesResult>
    {
        public async Task<GetRulesResult> Handle(GetRulesQuery request, CancellationToken cancellationToken)
        {
            var globalRules = await globalRulesService.GetAllRules();

            if (globalRules.Any())
            {
                return new GetRulesResult
                {
                    GlobalRules = globalRules
                };
            }

            var rules = await rulesService.GetRules();

            return new GetRulesResult
            {
                Rules = rules
            };
        }
    }
}
