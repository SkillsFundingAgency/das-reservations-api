using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands
{
    public class CreateAccountReservationValidator : IValidator<CreateAccountReservationCommand>
    {
        public Task<ValidationResult> ValidateAsync(CreateAccountReservationCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId == 0)
            {
                validationResult.AddError(nameof(item.AccountId));
            }
            if (item.StartDate == DateTime.MinValue)
            {
                validationResult.AddError(nameof(item.StartDate));
            }

            return Task.FromResult(validationResult);
        }
    }
}
