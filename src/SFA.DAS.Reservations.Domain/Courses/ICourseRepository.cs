using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Courses
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Entities.Course>> GetCourses();
        Task<Entities.Course> GetCourseById(string id);
    }
}
