using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Application.Rules.Queries.GetAvailableDates;
using SFA.DAS.Reservations.Domain.Courses;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.Rules.Queries;

public class GetAvailableDatesQueryHandler(
    IValidator<GetAvailableDatesQuery> validator,
    IAvailableDatesService availableDatesService,
    ICourseService courseService,
    IAvailableDatesRestrictionProvider restrictionProvider)
    : IRequestHandler<GetAvailableDatesQuery, GetAvailableDatesResult>
{
    public async Task<GetAvailableDatesResult> Handle(GetAvailableDatesQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid())
        {
            throw new ArgumentException(
                "The following parameters have failed validation",
                validationResult.ValidationDictionary.Select(pair => pair.Key)
                    .Aggregate((item1, item2) => $"{item1}, {item2}"));
        }

        var includePreviousMonth = true;
        if (!string.IsNullOrWhiteSpace(request.CourseId))
        {
            var course = await courseService.GetCourseById(request.CourseId.Trim());
            if (course != null)
            {
                includePreviousMonth = restrictionProvider.IncludePreviousMonth(course.LearningType);
            }
        }

        var availableDates = availableDatesService.GetAvailableDates(includePreviousMonth);

        return new GetAvailableDatesResult
        {
            AvailableDates = availableDates
        };
    }
}