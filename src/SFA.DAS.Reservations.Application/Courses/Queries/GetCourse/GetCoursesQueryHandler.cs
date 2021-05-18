using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Application.Courses.Queries.GetCourse.SFA.DAS.Reservations.Application.Courses.Queries.GetCourses;
using SFA.DAS.Reservations.Domain.Courses;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;
using System;

namespace SFA.DAS.Reservations.Application.Courses.Queries.GetCourses
{
    public class GetCourseQueryHandler : IRequestHandler<GetCourseQuery, Course>
    {
        private readonly ICourseService _service;

        public GetCourseQueryHandler(ICourseService service)
        {
            _service = service;
        }

        public async Task<Course> Handle(GetCourseQuery request, CancellationToken cancellationToken)
        {
            var course = await _service.GetCourseById(request.CourseId);

            if (course != null &&
                (course.EffectiveTo == null || course.EffectiveTo > DateTime.UtcNow)
               && !course.CourseId.Contains('-'))
                return course;

            return null;
        }
    }
}
