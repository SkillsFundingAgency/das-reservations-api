using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.Courses;
using Course = SFA.DAS.Reservations.Domain.Entities.Course;


namespace SFA.DAS.Reservations.Data.Repository
{
    public class CourseRepository(IReservationsDataContext reservationsDataContext) : ICourseRepository
    {
        public async Task<IEnumerable<Course>> GetCourses()
        {
            return await reservationsDataContext.Courses
                .Where(x => x.EffectiveTo == null || x.EffectiveTo > DateTime.UtcNow)
                .Where(x => !x.CourseId.Contains("-"))
                .ToListAsync();
        }

        public async Task<Course> GetCourseById(string id)
        {
            return await reservationsDataContext.Courses.FindAsync(id);
        }
    }
}
