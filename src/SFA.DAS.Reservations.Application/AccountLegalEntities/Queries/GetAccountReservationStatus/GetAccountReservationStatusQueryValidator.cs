using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus
{
    public class GetAccountReservationStatusQueryValidator : IValidator<GetAccountReservationStatusQuery>
    {
        public Task<ValidationResult> ValidateAsync(GetAccountReservationStatusQuery query)
        {
            var validationResult = new ValidationResult();

            if (query.AccountId == 0)
            {
                validationResult.AddError(nameof(query.AccountId));
            }

            return Task.FromResult(validationResult);
        }
    }
}