using System;

namespace SFA.DAS.Reservations.Domain.ApprenticeshipCourse
{
    public class Course
    {
        public string CourseId { get; set; }
        public string Title { get; set; }
        public string Level { get; set; }
        public string CourseDescription => string.IsNullOrEmpty(Level) 
            ? Title : $"{Title} - Level {Level}";

        public ApprenticeshipType Type => CourseId.IndexOf("-", StringComparison.CurrentCultureIgnoreCase) != -1
            ? ApprenticeshipType.Framework
            : ApprenticeshipType.Standard;
    }
}
