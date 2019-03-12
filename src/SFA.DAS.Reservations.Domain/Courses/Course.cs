using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Domain.Courses
{
    public class Course
    {
        public string Id { get;  }
        public string Title { get;  }
        public int Level { get; }
        public ICollection<ReservationRule> ReservationRules { get;  }

        public Course(Entities.Course entity)
        {
            Id = entity.CourseId;
            Title = entity.Title;
            Level = entity.Level;
            ReservationRules = new List<ReservationRule>();
        }
    }
}
