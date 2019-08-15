using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    public class WhenGettingCoursesFromRepository
    {
        private Course _activeCourse1;
        private Course _activeCourse2;
        private Course _expiredCourse1;
        private Course _expiredCourse2;
        private Mock<IReservationsDataContext> _reservationsDataContext;
        private CourseRepository _courseRepository;
        private List<Course> _expectedCourses;

        [SetUp]
        public void Arrange()
        {
            _activeCourse1 = new Course
            {
                CourseId = "1",
                Title = "Course 1",
                Level = 1,
                EffectiveTo = DateTime.Now.AddHours(4)
            };

            _activeCourse2 = new Course
            {
                CourseId = "2",
                Title = "Course 2",
                Level = 2,
                EffectiveTo = DateTime.Now.AddHours(1)
            };

            _expiredCourse1 = new Course
            {
                CourseId = "3",
                Title = "Course 3",
                Level = 3,
                EffectiveTo = DateTime.Now.AddHours(-1)
            };

            _expiredCourse2 = new Course
            {
                CourseId = "4",
                Title = "Course 4",
                Level = 3,
                EffectiveTo = DateTime.Now.AddHours(-3)
            };

            _expectedCourses = new List<Course>{ _activeCourse1, _expiredCourse2, _activeCourse2, _expiredCourse1};
            _reservationsDataContext = new Mock<IReservationsDataContext>();
            _reservationsDataContext.Setup(x => x.Courses).ReturnsDbSet(_expectedCourses);
            _courseRepository = new CourseRepository(_reservationsDataContext.Object);
        }

        [Test, MoqAutoData]
        public async Task ThenNonExpiredCoursesAreReturned() { 

            var result = await _courseRepository.GetCourses();

            Assert.True(result.Count() == 2);
            Assert.True(result.Contains(_activeCourse1));
            Assert.True(result.Contains(_activeCourse2));
            Assert.False(result.Contains(_expiredCourse1));
            Assert.False(result.Contains(_expiredCourse2));
        }
    }
}
