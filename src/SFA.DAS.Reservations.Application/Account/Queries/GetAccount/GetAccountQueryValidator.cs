using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.Account.Queries.GetAccount
{
    public class GetAccountQueryValidator : IValidator<GetAccountQuery>
    {
        public Task<ValidationResult> ValidateAsync(GetAccountQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.Id.Equals(0))
            {
                validationResult.AddError(nameof(item.Id));
            }
            
            return Task.FromResult(validationResult);
        }
    }
}