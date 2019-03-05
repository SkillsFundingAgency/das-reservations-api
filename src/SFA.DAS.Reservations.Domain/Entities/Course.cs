using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class Course
    {
        public string CourseId { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public virtual IList<Rule> ReservationRule { get; set; }
    }
}
