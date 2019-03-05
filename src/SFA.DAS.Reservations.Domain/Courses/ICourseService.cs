using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Courses
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetCourses();
    }
}
