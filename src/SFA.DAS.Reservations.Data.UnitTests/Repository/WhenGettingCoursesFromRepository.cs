using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    public class WhenGettingCoursesFromRepository
    {
        private Mock<IReservationsDataContext> _reservationsDataContext;
        private CourseRepository _courseRepository;
        private List<Course> _expectedCourses;

        [SetUp]
        public void Arrange()
        {
            _expectedCourses = new List<Course>
            {
                new Course
                {
                    CourseId = "1",
                    Level = 1,
                    ReservationRule = new List<Rule>(),
                    Title = "Course 1"
                },
                new Course
                {
                    CourseId = "2",
                    Level = 2,
                    ReservationRule = new List<Rule>(),
                    Title = "Course 2"
                },
                new Course
                {
                    CourseId = "3",
                    Level = 3,
                    ReservationRule = new List<Rule>(),
                    Title = "Course 3"
                }
            };

            _reservationsDataContext = new Mock<IReservationsDataContext>();
            _reservationsDataContext.Setup(x => x.Courses).ReturnsDbSet(_expectedCourses);

            _courseRepository = new CourseRepository(_reservationsDataContext.Object);
        }

        [Test]
        public async Task Then_Courses_Should_Be_Returned()
        {
            //Act
            var actual = await _courseRepository.GetCourses();

            //Assert
            Assert.AreEqual(_expectedCourses, actual);
        }
    }
}
