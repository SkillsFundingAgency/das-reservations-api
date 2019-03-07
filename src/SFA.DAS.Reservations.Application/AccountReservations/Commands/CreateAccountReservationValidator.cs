using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Courses;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands
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

            if (item.AccountId == 0)
            {
                validationResult.AddError(nameof(item.AccountId));
            }
            if (item.StartDate == DateTime.MinValue)
            {
                validationResult.AddError(nameof(item.StartDate));
            }

            if (string.IsNullOrEmpty(item.CourseId))
            {
                return validationResult;
            }
                
            if (await _courseService.GetCourseById(item.CourseId) == null)
            {
                validationResult.AddError(nameof(item.CourseId), "Course with CourseId cannot be found");
            }

            return validationResult;
        }
    }
}
