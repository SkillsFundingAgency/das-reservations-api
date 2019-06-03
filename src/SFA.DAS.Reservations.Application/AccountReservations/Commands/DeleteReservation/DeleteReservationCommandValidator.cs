using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.DeleteReservation
{
    public class DeleteReservationCommandValidator : IValidator<DeleteReservationCommand>
    {
        public Task<ValidationResult> ValidateAsync(DeleteReservationCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.ReservationId == Guid.Empty)
            {
                validationResult.AddError(nameof(item.ReservationId));
            }

            return Task.FromResult(validationResult);
        }
    }
}