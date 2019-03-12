using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Courses
{
    public interface ICourseService
    {
        Task<IEnumerable<ApprenticeshipCourse.Course>> GetCourses();
        Task<ApprenticeshipCourse.Course> GetCourseById(string id);
    }
}
