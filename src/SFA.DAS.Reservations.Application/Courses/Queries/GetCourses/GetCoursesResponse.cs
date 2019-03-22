using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;

namespace SFA.DAS.Reservations.Application.Courses.Queries.GetCourses
{
    public class GetCoursesResponse
    {
        public IEnumerable<Course> Courses { get; set; }
    }
}
