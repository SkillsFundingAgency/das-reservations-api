using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Domain.Courses
{
    public class Course
    {
        public string Id { get;  }
        public string Title { get;  }
        public int Level { get; }
        public IList<ReservationRule> ReservationRules { get;  }

        public Course(string id, string title, int level)
        {
            Id = id;
            Title = title;
            Level = level;
            ReservationRules = new List<ReservationRule>();
        }
    }
}
