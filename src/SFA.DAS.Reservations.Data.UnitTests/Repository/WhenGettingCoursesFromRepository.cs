using System;
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
    public class WhenGettingCoursesFromRepository
    {
        private Course _activeCourse1;
        private Course _activeCourse2;
        private Course _activeCourse3;
        private Course _expiredCourse1;
        private Course _expiredCourse2;
        private Course _frameworkCourse;
        private Mock<IReservationsDataContext> _reservationsDataContext;
        private CourseRepository _courseRepository;
        private List<Course> _courses;

        [SetUp]
        public void Arrange()
        {
            _activeCourse1 = new Course
            {
                CourseId = "1",
                Title = "Course 1",
                Level = 1,
                EffectiveTo = DateTime.UtcNow.AddHours(4)
            };

            _activeCourse2 = new Course
            {
                CourseId = "2",
                Title = "Course 2",
                Level = 2,
                EffectiveTo = DateTime.UtcNow.AddHours(1)
            };


            _activeCourse3 = new Course
            {
                CourseId = "2",
                Title = "Course 2",
                Level = 2,
                EffectiveTo = null
            };

            _expiredCourse1 = new Course
            {
                CourseId = "3",
                Title = "Course 3",
                Level = 3,
                EffectiveTo = DateTime.UtcNow.AddHours(-1)
            };

            _expiredCourse2 = new Course
            {
                CourseId = "4",
                Title = "Course 4",
                Level = 3,
                EffectiveTo = DateTime.UtcNow.AddHours(-3)
            };

            _frameworkCourse = new Course
            {
                CourseId = "5-1",
                Title = "Framework 1",
                Level = 3,
                EffectiveTo = DateTime.UtcNow.AddHours(4)
            };

            _courses = new List<Course> { _activeCourse1, _expiredCourse2, _activeCourse3, _activeCourse2, _expiredCourse1, _frameworkCourse };
            _reservationsDataContext = new Mock<IReservationsDataContext>();
            _reservationsDataContext.Setup(x => x.Courses).ReturnsDbSet(_courses);
            _courseRepository = new CourseRepository(_reservationsDataContext.Object);
        }

        [Test]
        public async Task ThenNonExpiredCoursesAreReturned()
        {

            var result = (await _courseRepository.GetCourses()).ToList();

            Assert.IsTrue(result.Count == 3);
            Assert.IsTrue(result.Contains(_activeCourse1));
            Assert.IsTrue(result.Contains(_activeCourse2));
            Assert.IsTrue(result.Contains(_activeCourse3));
            Assert.IsFalse(result.Contains(_expiredCourse1));
            Assert.IsFalse(result.Contains(_expiredCourse2));
        }

        [Test]
        public async Task ThenFrameworkCoursesAreNotReturned()
        {
            var result = (await _courseRepository.GetCourses()).ToList();

            Assert.IsFalse(result.Contains(_frameworkCourse));
        }

    }
}
