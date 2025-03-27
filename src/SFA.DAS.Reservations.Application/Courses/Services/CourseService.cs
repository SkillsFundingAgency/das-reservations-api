using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Courses;
using Course = SFA.DAS.Reservations.Domain.ApprenticeshipCourse.Course;


namespace SFA.DAS.Reservations.Application.Courses.Services
{
    public class CourseService(ICourseRepository repository) : ICourseService
    {
        public async Task<IEnumerable<Course>> GetCourses()
        {
            var courseEntities = await repository.GetCourses();

            return courseEntities.Select(entity => new Course(entity)).ToArray();
        }

        public async Task<Course> GetCourseById(string id)
        {
            var entity = await repository.GetCourseById(id);

            return entity == null ? null : new Course(entity);
        }
    }
}
