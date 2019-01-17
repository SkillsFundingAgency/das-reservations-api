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

            if (item.Reservation.AccountId == 0)
            {
                validationResult.AddError(nameof(item.Reservation.AccountId));
            }
            if (item.Reservation.StartDate == DateTime.MinValue)
            {
                validationResult.AddError(nameof(item.Reservation.StartDate));
            }

            return Task.FromResult(validationResult);
        }
    }
}
