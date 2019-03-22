using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;

namespace SFA.DAS.Reservations.Api.Models
{
    public class CoursesViewModel
    {
        public IEnumerable<Course> Courses { get; }

        public CoursesViewModel(IEnumerable<Course> courses)
        {
            Courses = courses;
        }
    }
}
