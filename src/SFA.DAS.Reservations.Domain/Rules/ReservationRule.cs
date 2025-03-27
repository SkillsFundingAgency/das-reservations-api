using System;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public class ReservationRule(Entities.Rule rule)
    {
        public long Id { get;  } = rule.Id;
        public DateTime CreatedDate { get;  } = rule.CreatedDate;
        public DateTime ActiveFrom { get;  } = rule.ActiveFrom;
        public DateTime ActiveTo { get;  } = rule.ActiveTo;
        public string CourseId { get;  } = rule.CourseId;
        public AccountRestriction Restriction { get;  } = (AccountRestriction) rule.Restriction;
        public Course Course { get;  } = new(rule.Course.CourseId, rule.Course.Title, rule.Course.Level.ToString(), rule.Course.EffectiveTo, rule.Course.ApprenticeshipType);
    }
}
