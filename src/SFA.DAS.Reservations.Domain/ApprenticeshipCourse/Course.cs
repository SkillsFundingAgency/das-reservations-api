using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Domain.ApprenticeshipCourse
{
    public class Course
    {
        public Course(Entities.Course entity)
        {
            CourseId = entity.CourseId;
            Title = entity.Title;
            Level = entity.Level.ToString();
            EffectiveTo = entity.EffectiveTo;
            Rules = entity.ReservationRule;
        }

        public Course(string courseId, string title, string level, DateTime? effectiveTo)
        {
            CourseId = courseId;
            Title = title;
            Level = level;
            EffectiveTo = effectiveTo;
            Rules = new List<Rule>();
        }

        public string CourseId { get; }
        public string Title { get;}
        public string Level { get; }
        public DateTime? EffectiveTo { get; set; }
        public ApprenticeshipType Type => 
            CourseId.IndexOf("-", StringComparison.CurrentCultureIgnoreCase) != -1
            ? ApprenticeshipType.Framework
            : ApprenticeshipType.Standard;
        public string StandardApprenticeshipType { get; }
        public ICollection<Rule> Rules { get; }
      
        public IEnumerable<Rule> GetActiveRules(ReservationDates dates)
        {
            return Rules.Where(r => 
                r.CreatedDate <= dates.ReservationCreatedDate &&
                r.ActiveFrom <= dates.ReservationExpiryDate && 
                r.ActiveTo >= dates.ReservationStartDate);
        }
    }
}
