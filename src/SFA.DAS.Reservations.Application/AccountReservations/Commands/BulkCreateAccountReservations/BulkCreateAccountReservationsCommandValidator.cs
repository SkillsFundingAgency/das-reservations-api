using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations
{
    public class BulkCreateAccountReservationsCommandValidator : IValidator<BulkCreateAccountReservationsCommand>
    {
        public Task<ValidationResult> ValidateAsync(BulkCreateAccountReservationsCommand command)
        {
            var validationResult = new ValidationResult();

            if (command.ReservationCount == default(uint))
            {
                validationResult.AddError(nameof(command.ReservationCount), $"{nameof(command.ReservationCount)} has not be set");
            }

            return Task.FromResult(validationResult);
        }
    }
}
