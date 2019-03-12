using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Courses;
using Course = SFA.DAS.Reservations.Domain.ApprenticeshipCourse.Course;


namespace SFA.DAS.Reservations.Application.Courses.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;

        public CourseService(ICourseRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Course>> GetCourses()
        {
            var courseEntities = await _repository.GetCourses();

            return courseEntities.Select(entity => new Course(entity)).ToArray();
        }

        public async Task<Course> GetCourseById(string id)
        {
            var entity = await _repository.GetCourseById(id);

            return entity == null ? null : new Course(entity);
        }
    }
}
