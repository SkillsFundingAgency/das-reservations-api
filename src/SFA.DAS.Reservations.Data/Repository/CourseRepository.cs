using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class CourseRepository
    {
        private readonly IReservationsDataContext _reservationsDataContext;

        public CourseRepository(IReservationsDataContext reservationsDataContext)
        {
            _reservationsDataContext = reservationsDataContext;
        }

        public async Task<IEnumerable<Course>> GetCourses()
        {
            return await _reservationsDataContext.Courses.ToArrayAsync();
        }
    }
}
