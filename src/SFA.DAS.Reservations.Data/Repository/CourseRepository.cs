using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.Courses;
using Course = SFA.DAS.Reservations.Domain.Entities.Course;


namespace SFA.DAS.Reservations.Data.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly IReservationsDataContext _reservationsDataContext;

        public CourseRepository(IReservationsDataContext reservationsDataContext)
        {
            _reservationsDataContext = reservationsDataContext;
        }

        public async Task<IEnumerable<Course>> GetCourses()
        {

            return  await _reservationsDataContext.Courses.Where(x => x.EffectiveTo > DateTime.UtcNow).ToListAsync();
        }

        public async Task<Course> GetCourseById(string id)
        {
            return await _reservationsDataContext.Courses.FindAsync(id);
        }
    }
}
