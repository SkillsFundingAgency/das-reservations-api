using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Courses;

namespace SFA.DAS.Reservations.Application.Courses.Queries.GetCourses
{
    public class GetCoursesQueryHandler(ICourseService service) : IRequestHandler<GetCoursesQuery, GetCoursesResponse>
    {
        public async Task<GetCoursesResponse> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = await service.GetCourses();

            return new GetCoursesResponse
            {
                Courses = courses
            };
        }
    }
}
