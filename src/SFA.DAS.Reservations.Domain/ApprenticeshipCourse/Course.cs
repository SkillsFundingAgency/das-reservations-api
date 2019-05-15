using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Domain.ApprenticeshipCourse
{
    public class Course
    {

        public Course(Entities.Course entity)
        {
            CourseId = entity.CourseId;
            Title = entity.Title;
            Level = entity.Level.ToString();
            Rules = entity.ReservationRule;
        }

        public Course(string courseId, string title, string level)
        {
            CourseId = courseId;
            Title = title;
            Level = level;
            Rules = new List<Rule>();
        }
        public string CourseId { get; }
        public string Title { get;}
        public string Level { get; }
        public ApprenticeshipType Type => 
            CourseId.IndexOf("-", StringComparison.CurrentCultureIgnoreCase) != -1
            ? ApprenticeshipType.Framework
            : ApprenticeshipType.Standard;

        public ICollection<Rule> Rules { get; }

        public IEnumerable<Rule> GetActiveRules()
        {
            return Rules.Where(r => r.ActiveFrom <= DateTime.Now && r.ActiveTo >= DateTime.Now);
        }
    }
}
