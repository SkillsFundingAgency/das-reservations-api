using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.Rules.Queries.GetAvailableDates;

public class GetAvailableDatesValidator : IValidator<GetAvailableDatesQuery>
{
    public Task<ValidationResult> ValidateAsync(GetAvailableDatesQuery item)
    {
        var validationResult = new ValidationResult();

        if (item.AccountLegalEntityId < 1)
        {
            validationResult.AddError(nameof(item.AccountLegalEntityId));
        }

        return Task.FromResult(validationResult);
    }
}