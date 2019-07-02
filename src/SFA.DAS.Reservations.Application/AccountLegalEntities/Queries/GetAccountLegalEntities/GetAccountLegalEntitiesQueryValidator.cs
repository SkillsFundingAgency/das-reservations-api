using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntities
{
    public class GetAccountLegalEntitiesQueryValidator : IValidator<GetAccountLegalEntitiesQuery>
    {
        public Task<ValidationResult> ValidateAsync(GetAccountLegalEntitiesQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId == 0)
            {
                validationResult.AddError(nameof(item.AccountId));
            }

            return Task.FromResult(validationResult);
        }
    }
}