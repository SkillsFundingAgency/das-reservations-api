using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class GetReservationValidator : IValidator<GetReservationQuery>
    {
        public Task<ValidationResult> ValidateAsync(GetReservationQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.Id == Guid.Empty)
            {
                validationResult.AddError(nameof(item.Id));
            }

            return Task.FromResult(validationResult);
        }
    }
}