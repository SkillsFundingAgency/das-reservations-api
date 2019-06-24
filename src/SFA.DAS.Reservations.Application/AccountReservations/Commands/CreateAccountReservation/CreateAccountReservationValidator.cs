using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Courses;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation
{
    public class CreateAccountReservationValidator : IValidator<CreateAccountReservationCommand>
    {
        private readonly ICourseService _courseService;

        public CreateAccountReservationValidator(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<ValidationResult> ValidateAsync(CreateAccountReservationCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.Id == Guid.Empty)
            {
                validationResult.AddError(nameof(item.Id));
            }
            if (item.AccountId == 0)
            {
                validationResult.AddError(nameof(item.AccountId));
            }

            if (item.AccountLegalEntityId == 0)
            {
                validationResult.AddError(nameof(item.AccountLegalEntityId));
            }

            if (!item.IsLevyAccount)
            {
                if (string.IsNullOrEmpty(item.AccountLegalEntityName))
                {
                    validationResult.AddError(nameof(item.AccountLegalEntityName));
                }
                if (string.IsNullOrEmpty(item.CourseId))
                {
                    validationResult.AddError(nameof(item.CourseId));
                }
                if (!item.StartDate.HasValue || item.StartDate == DateTime.MinValue)
                {
                    validationResult.AddError(nameof(item.StartDate));
                }
                if (validationResult.IsValid() && await _courseService.GetCourseById(item.CourseId) == null)
                {
                    validationResult.AddError(nameof(item.CourseId), "Course with CourseId cannot be found");
                }
            }

            return validationResult;
        }
    }
}
