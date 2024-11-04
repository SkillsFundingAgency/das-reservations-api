using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Courses.Queries.GetCourses;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;
using SFA.DAS.Reservations.Domain.Courses;

namespace SFA.DAS.Reservations.Application.UnitTests.Courses.Queries.GetCourses
{
    public class WhenIGetCourses
    {
        private GetCoursesQueryHandler _handler;
        private GetCoursesQuery _query;
        private List<Course> _expectedCourses;
        private Mock<ICourseService> _service;

        [SetUp]
        public void Arrange()
        {
            _expectedCourses = new List<Course>
            {
                new Course(new Domain.Entities.Course
                {
                    CourseId = "1", 
                    Title = "Course 1", 
                    Level = 1
                }),
                new Course(new Domain.Entities.Course
                {
                    CourseId = "2", 
                    Title = "Course 2", 
                    Level = 2
                }),
                new Course(new Domain.Entities.Course
                {
                    CourseId = "3", 
                    Title = "Course 3", 
                    Level = 3
                })
            };

            _service = new Mock<ICourseService>();
            _service.Setup(s => s.GetCourses()).ReturnsAsync(_expectedCourses);

            _query = new GetCoursesQuery();
            _handler = new GetCoursesQueryHandler(_service.Object);
        }

        [Test]
        public async Task The_Courses_Should_Be_Returned()
        {
            //Act
            var response = await _handler.Handle(_query, CancellationToken.None);

            //Assert
            response.Courses.Should().BeEquivalentTo(_expectedCourses);
        }
    }
}
