using System;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public class ReservationRule
    {
        public ReservationRule(Entities.Rule rule)
        {
            CreatedDate = rule.CreatedDate;
            ActiveFrom = rule.ActiveFrom;
            ActiveTo = rule.ActiveTo;
            Id = rule.Id;
            Restriction = (AccountRestriction) rule.Restriction;
            CourseId = rule.CourseId;
            Course = new Course(rule.Course.CourseId, rule.Course.Title, rule.Course.Level.ToString());
        }

        public long Id { get;  }
        public DateTime CreatedDate { get;  }
        public DateTime ActiveFrom { get;  }
        public DateTime ActiveTo { get;  }
        public string CourseId { get;  }
        public AccountRestriction Restriction { get;  }
        public Course Course { get;  }
    }
}
