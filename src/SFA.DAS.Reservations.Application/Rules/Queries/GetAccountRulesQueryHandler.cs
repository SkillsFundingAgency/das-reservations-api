using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetAccountRulesQueryHandler : IRequestHandler<GetAccountRulesQuery, GetAccountRulesResult>
    {
        private readonly IGlobalRulesService _globalRulesService;
        private readonly IValidator<GetAccountRulesQuery> _validator;

        public GetAccountRulesQueryHandler(IGlobalRulesService globalRulesService, IValidator<GetAccountRulesQuery> validator)
        {
            _globalRulesService = globalRulesService;
            _validator = validator;
        }

        public async Task<GetAccountRulesResult> Handle(GetAccountRulesQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation", validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var result = (await _globalRulesService.GetAccountRules(request.AccountId)).ToList();
            var globalRuleResult = await _globalRulesService.GetActiveRules(DateTime.UtcNow);

            result.AddRange(globalRuleResult);
            result = result.Where(r => r?.Exceptions == null || r.Exceptions.All(e => e != request.AccountId)).ToList();

            return new GetAccountRulesResult
            {
                GlobalRules = result
            };
        }
    }
}