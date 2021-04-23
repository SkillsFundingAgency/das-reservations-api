using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.Reservations.Application.Courses.Queries.GetCourses;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;

namespace SFA.DAS.Reservations.Application.Courses.Queries.GetCourse
{


    namespace SFA.DAS.Reservations.Application.Courses.Queries.GetCourses
    {
        public class GetCourseQuery : IRequest<Course>
        {
            public string CourseId { get; set; }
        }
    }

}
