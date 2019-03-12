using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Courses;

namespace SFA.DAS.Reservations.Application.Courses.Queries.GetCourses
{
    public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, GetCoursesResponse>
    {
        private readonly ICourseService _service;

        public GetCoursesQueryHandler(ICourseService service)
        {
            _service = service;
        }

        public async Task<GetCoursesResponse> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = await _service.GetCourses();

            return new GetCoursesResponse
            {
                Courses = courses
            };
        }
    }
}
