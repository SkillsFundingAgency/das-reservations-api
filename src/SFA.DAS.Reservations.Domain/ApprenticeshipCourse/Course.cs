using System;

namespace SFA.DAS.Reservations.Domain.ApprenticeshipCourse
{
    public class Course
    {

        public Course(Entities.Course entity)
        {
            CourseId = entity.CourseId;
            Title = entity.Title;
            Level = entity.Level.ToString();
        }

        public Course(string courseId, string title, string level)
        {
            CourseId = courseId;
            Title = title;
            Level = level;
        }
        public string CourseId { get; }
        public string Title { get;}
        public string Level { get; }
        public string CourseDescription => string.IsNullOrEmpty(Level) 
            ? Title : $"{Title} - Level {Level}";

        public ApprenticeshipType Type => 
            CourseId.IndexOf("-", StringComparison.CurrentCultureIgnoreCase) != -1
            ? ApprenticeshipType.Framework
            : ApprenticeshipType.Standard;
    }
}
