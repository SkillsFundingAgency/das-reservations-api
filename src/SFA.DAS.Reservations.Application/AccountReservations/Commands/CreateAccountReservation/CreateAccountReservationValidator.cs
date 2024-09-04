using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Courses;
using SFA.DAS.Reservations.Domain.Validation;
using StructureMap.Diagnostics;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation
{
    public class CreateAccountReservationValidator : IValidator<CreateAccountReservationCommand>
    {
        private readonly ICourseService _courseService;
        private readonly ICurrentDateTime _currentDateTime;

        public CreateAccountReservationValidator(ICourseService courseService, ICurrentDateTime currentDateTime)
        {
            _courseService = courseService;
            _currentDateTime = currentDateTime;
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
                else
                {
                    var currentDate = _currentDateTime.GetDate();
                    var currentFirstOfTheMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                    var validFromDate = currentDate.AddMonths(-1).ToString("MM yyyy");
                    var validToDate = currentDate.AddMonths(2).ToString("MM yyyy");
                    var errorMessage = $"Training start date must be between the funding reservation dates {validFromDate} to {validToDate}";
                    var startDate = item.StartDate.Value;
                    if (currentFirstOfTheMonth.AddMonths(-1) > startDate || currentFirstOfTheMonth.AddMonths(2) < startDate)
                    {
                        validationResult.AddError(nameof(item.StartDate), errorMessage);
                    }
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
