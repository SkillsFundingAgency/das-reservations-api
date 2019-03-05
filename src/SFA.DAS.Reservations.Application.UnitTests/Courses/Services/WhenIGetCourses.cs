using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Courses.Services;
using SFA.DAS.Reservations.Domain.Courses;

namespace SFA.DAS.Reservations.Application.UnitTests.Courses.Services
{
    public class WhenIGetCourses
    {
        private CourseService _service;
        private Mock<ICourseRepository> _repository;
        private List<Domain.Entities.Course> _courseEntities;
        private List<Course> _expectedCourses;

        [SetUp]
        public void Arrange()
        {
            _courseEntities = new List<Domain.Entities.Course>();
            _expectedCourses = new List<Course>();

            _repository = new Mock<ICourseRepository>();
            _service = new CourseService(_repository.Object);

            for (var index = 1; index <= 3; index++)
            {
                _courseEntities.Add(new Domain.Entities.Course
                {
                    CourseId = index.ToString(),
                    Title = $"Course {index}",
                    Level = index
                });

                _expectedCourses.Add(new Course(index.ToString(), $"Course {index}", index));
            }

            _repository.Setup(r => r.GetCourses()).ReturnsAsync(_courseEntities);
        }

        [Test]
        public async Task Then_Courses_Should_Be_Returned()
        {
            //Act
            var actual = await _service.GetCourses();

            //Assert
            actual.Should().BeEquivalentTo(_expectedCourses);
        }
    }
}
