using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Courses.Services;
using SFA.DAS.Reservations.Domain.Courses;

namespace SFA.DAS.Reservations.Application.UnitTests.Courses.Services
{
    public class WhenIGetACourse
    {
        private CourseService _service;
        private Mock<ICourseRepository> _repository;
        
        private Course _expectedCourse;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<ICourseRepository>();
            _service = new CourseService(_repository.Object);

            var entity = new Domain.Entities.Course
            {
                CourseId = "1",
                Title = "Course 1",
                Level = 1
            };

            _expectedCourse = new Course(entity);
            _repository.Setup(r => r.GetCourseById(It.IsAny<string>())).ReturnsAsync(entity);
        }

        [Test]
        public async Task Then_Course_Should_Be_Returned_If_Exists()
        {
            //Act
            var actual = await _service.GetCourseById("2");

            //Assert
            actual.Should().BeEquivalentTo(_expectedCourse);
        }

        [Test]
        public async Task Then_Null_Should_Be_Returned_If_Course_Does_Not_Exists()
        {
            //Assign
            _repository.Setup(r => r.GetCourseById(It.IsAny<string>())).ReturnsAsync((Domain.Entities.Course) null);

            //Act
            var actual = await _service.GetCourseById("12");

            //Assert
            Assert.IsNull(actual);
        }
    }
}
