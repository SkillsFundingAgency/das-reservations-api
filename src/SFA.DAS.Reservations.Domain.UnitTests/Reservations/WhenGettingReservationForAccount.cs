using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using Reservation = SFA.DAS.Reservations.Domain.Reservations.Reservation;

namespace SFA.DAS.Reservations.Domain.UnitTests.Reservations
{
    public class WhenGettingReservationForAccount
    {
        private Reservation _reservation;
        private Course _expectedCourse;
        private Func<DateTime, Task<IList<Rule>>> _rules;

        [SetUp]
        public void Arrange()
        {
            _expectedCourse = new Course
            {
                CourseId = "123-4",
                Level = 1,
                Title = "Test Course"
            };
            _rules = (startDate) =>
            {
                var rules = new List<Rule>
                    {
                        new Rule
                        {
                            CourseId = "1",
                            Course = _expectedCourse,
                            Restriction = 0
                        },
                        new Rule
                        {
                            CourseId = "1",
                            Course = _expectedCourse,
                            Restriction = 1
                        },
                        new Rule
                        {
                            CourseId = "1",
                            Course = _expectedCourse,
                            Restriction = 2
                        }
                    };
                return Task.FromResult((IList<Rule>)rules);
            };
        }

        [Test]
        public void Then_The_Rules_For_That_Reservation_Period_Are_Taken_From_The_Repository_And_Filtered_By_Account_Type()
        {
            //Act
            _reservation = new Reservation(_rules, 1, 1, true, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow.AddDays(-1), ReservationStatus.Pending);
            
            //Assert
            Assert.AreEqual(2, _reservation.Rules.Count);
            Assert.IsTrue(_reservation.Rules.Any(c => c.Restriction.Equals(AccountRestriction.All)));
            Assert.IsTrue(_reservation.Rules.Any(c => c.Restriction.Equals(AccountRestriction.Levy)));
        }

        [Test]
        public void Then_The_Reservation_Is_Valid_If_It_Is_Within_The_Expiry_Period()
        {
            //Act
            _reservation = new Reservation(null, 1, 1, false, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), ReservationStatus.Pending);

            //Assert
            Assert.IsTrue(_reservation.IsActive);
            
        }

        [Test]
        public void Then_The_Reservation_Is_Not_Valid_If_It_Has_Fallen_Out_Of_The_Expiry_Period()
        {
            //Act
            _reservation = new Reservation(null, 1, 1, false, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow.AddDays(-1), ReservationStatus.Pending);

            //Assert
            Assert.IsFalse(_reservation.IsActive);
        }

        [Test]
        public void Then_The_Associated_Course_Is_Included()
        {
            //Act
            _reservation = new Reservation(_rules, 1, 1, true, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow.AddDays(-1), ReservationStatus.Pending);

            //Assert
            Assert.AreEqual(2, _reservation.Rules.Count);
            Assert.IsTrue(_reservation.Rules.All(c => c.Course != null));
            var actualRuleCourse = _reservation.Rules.FirstOrDefault()?.Course;
            Assert.IsNotNull(actualRuleCourse);
            Assert.AreEqual(_expectedCourse.CourseId, actualRuleCourse.CourseId);
            Assert.AreEqual(_expectedCourse.Title, actualRuleCourse.Title);
            Assert.AreEqual(_expectedCourse.Level.ToString(), actualRuleCourse.Level);
        }
    }
}
