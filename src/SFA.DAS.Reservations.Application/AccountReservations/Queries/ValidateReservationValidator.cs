using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class ValidateReservationValidator : IValidator<ValidateReservationQuery>
    {
        public Task<ValidationResult> ValidateAsync(ValidateReservationQuery query)
        {
            var validationResult = new ValidationResult();
            
            if(query.ReservationId.Equals(Guid.Empty))
            {
                validationResult.AddError(nameof(query.ReservationId));
            }

            if(string.IsNullOrEmpty(query.CourseId))
            {
                validationResult.AddError(nameof(query.CourseId));
            }

            if(query.TrainingStartDate.Equals(DateTime.MinValue))
            {
                validationResult.AddError(nameof(query.TrainingStartDate));
            }

            return Task.FromResult(validationResult);
        }
    }
}
