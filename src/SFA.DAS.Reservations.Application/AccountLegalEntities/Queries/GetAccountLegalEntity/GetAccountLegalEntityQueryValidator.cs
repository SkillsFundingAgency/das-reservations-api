using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntity
{
    public class GetAccountLegalEntityQueryValidator : IValidator<GetAccountLegalEntityQuery>
    {
        public Task<ValidationResult> ValidateAsync(GetAccountLegalEntityQuery query)
        {
            var validationResult = new ValidationResult();

            if (query.Id == default(long))
            {
                validationResult.AddError(nameof(query.Id));
            }

            return Task.FromResult(validationResult);
        }
    }
}
