using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.ProviderPermissions.Queries
{
    public class GetAccountLegalEntitiesForProviderValidator : IValidator<GetAccountLegalEntitiesForProviderQuery>
    {
        public Task<ValidationResult> ValidateAsync(GetAccountLegalEntitiesForProviderQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.ProviderId.Equals(0))
            {
                validationResult.AddError(nameof(item.ProviderId));
            }

            return Task.FromResult(validationResult);
        }
    }
}