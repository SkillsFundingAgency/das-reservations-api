using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Types;

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
            StandardApprenticeshipType = entity.ApprenticeshipType;
            LearningType = entity.LearningType;
        }

        public Course(string courseId, string title, string level, DateTime? effectiveTo, string apprenticeshipType, LearningType? learningType = null)
        {
            CourseId = courseId;
            Title = title;
            Level = level;
            EffectiveTo = effectiveTo;
            Rules = new List<Rule>();
            StandardApprenticeshipType = apprenticeshipType;
            LearningType = learningType;
        }

        public string CourseId { get; }
        public string Title { get;}
        public string Level { get; }
        public DateTime? EffectiveTo { get; set; }
        public ApprenticeshipType Type =>
            CourseId.IndexOf("-", StringComparison.CurrentCultureIgnoreCase) != -1
            ? ApprenticeshipCourse.ApprenticeshipType.Framework
            : ApprenticeshipCourse.ApprenticeshipType.Standard;
        public string StandardApprenticeshipType { get; }
        public LearningType? LearningType { get; }
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
