using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;

namespace SFA.DAS.Reservations.Api.Models
{
    public class CoursesViewModel(IEnumerable<Course> courses)
    {
        public IEnumerable<Course> Courses { get; } = courses;
    }
}
