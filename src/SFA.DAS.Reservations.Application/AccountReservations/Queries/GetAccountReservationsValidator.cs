using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class GetAccountReservationsValidator : IValidator<GetAccountReservationsQuery>
    {
        public Task<ValidationResult> ValidateAsync(GetAccountReservationsQuery item)
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
