using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetAccountRulesValidator : IValidator<GetAccountRulesQuery>
    {
        public Task<ValidationResult> ValidateAsync(GetAccountRulesQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId <= 0)
            {
                validationResult.AddError(nameof(item.AccountId));
            }

            return Task.FromResult(validationResult);
        }
    }
}