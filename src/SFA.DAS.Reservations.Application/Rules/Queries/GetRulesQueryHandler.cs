using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetRulesQueryHandler : IRequestHandler<GetRulesQuery, GetRulesResult>
    {
        private readonly IRulesService _rulesService;

        public GetRulesQueryHandler(IRulesService rulesService)
        {
            _rulesService = rulesService;
        }

        public async Task<GetRulesResult> Handle(GetRulesQuery request, CancellationToken cancellationToken)
        {
            var rules = await _rulesService.GetRules();

            return new GetRulesResult
            {
                Rules = rules
            };
        }
    }
}
