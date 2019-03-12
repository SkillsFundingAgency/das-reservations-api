using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    public class WhenGettingACourses
    {
        private const string ExpectedCourseId = "2";

        private Mock<IReservationsDataContext> _reservationsDataContext;
        private CourseRepository _courseRepository;
        private List<Course> _courses;

        [SetUp]
        public void Arrange()
        {
           _courses = new List<Course>
           {
              new Course
               {
                   CourseId = "1",
                   Title = "Course 1",
                   Level = 1
               },
               new Course
               {
                   CourseId = "2",
                   Title = "Course 2",
                   Level = 2
               },
               new Course
               {
                   CourseId = "3",
                   Title = "Course 3",
                   Level = 3
               }
           };

            _reservationsDataContext = new Mock<IReservationsDataContext>();
            _reservationsDataContext.Setup(x => x.Courses.FindAsync(ExpectedCourseId))
                .ReturnsAsync(_courses.FirstOrDefault(c => c.CourseId.Equals(ExpectedCourseId)));

            _courseRepository = new CourseRepository(_reservationsDataContext.Object);
        }
        
        [Test]
        public async Task Then_Course_Should_Be_Returned_If_Exists()
        {
            //Act
            var actual = await _courseRepository.GetCourseById("2");

            //Assert
            Assert.AreEqual("2", actual.CourseId);
            Assert.AreEqual("Course 2", actual.Title);
            Assert.AreEqual(2, actual.Level);
        }

        [Test]
        public async Task Then_Null_Should_Be_Returned_If_Course_Does_Not_Exists()
        {
            //Act
            var actual = await _courseRepository.GetCourseById("5");

            //Assert
           Assert.IsNull(actual);
        }
    }
}
