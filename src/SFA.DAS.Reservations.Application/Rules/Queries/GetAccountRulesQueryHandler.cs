﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetAccountRulesQueryHandler(
        IGlobalRulesService globalRulesService,
        IValidator<GetAccountRulesQuery> validator)
        : IRequestHandler<GetAccountRulesQuery, GetAccountRulesResult>
    {
        public async Task<GetAccountRulesResult> Handle(GetAccountRulesQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation", validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var result = (await globalRulesService.GetAccountRules(request.AccountId)).ToList();
            var globalRuleResult = await globalRulesService.GetActiveRules(DateTime.UtcNow);

            result.AddRange(globalRuleResult);

            result = result.Where(r => r == null || !r.GlobalRuleAccountExemptions.Any(exemption => exemption.AccountId == request.AccountId)).ToList();

            return new GetAccountRulesResult
            {
                GlobalRules = result
            };
        }
    }
}