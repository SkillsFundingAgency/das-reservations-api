using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetGlobalRulesQueryHandler : IRequestHandler<GetGlobalRulesQuery, GetGlobalRulesResult>
    {
        private readonly IGlobalRulesService _service;

        public GetGlobalRulesQueryHandler(IGlobalRulesService service)
        {
            _service = service;
        }

        public async Task<GetGlobalRulesResult> Handle(GetGlobalRulesQuery request, CancellationToken cancellationToken)
        {
            var globalRules = await _service.GetRules();

            return new GetGlobalRulesResult {GlobalRules = globalRules};
        }
    }
}