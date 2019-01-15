using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using Apprenticeship = SFA.DAS.Reservations.Domain.Entities.Apprenticeship;
using Reservation = SFA.DAS.Reservations.Domain.Reservations.Reservation;

namespace SFA.DAS.Reservations.Domain.UnitTests.Reservations
{
    public class WhenGettingReservationForAccount
    {
        private Reservation _reservation;
        private Mock<IRuleRepository> _ruleRepository;
        private Apprenticeship _expectedApprenticeshipCourse;

        [SetUp]
        public void Arrange()
        {
            _ruleRepository = new Mock<IRuleRepository>();
            _expectedApprenticeshipCourse = new Apprenticeship
            {
                Id = 2,
                CourseId = "123-4",
                Level = 1,
                Title = "Test Course"
            };
            _ruleRepository.Setup(x=>x.GetReservationRules(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(new List<Rule>
            {
                new Rule
                {
                    ApprenticeshipId = 1,
                    ApprenticeshipCourse = _expectedApprenticeshipCourse,
                    Restriction = 0
                },
                new Rule
                {
                    ApprenticeshipId = 1,
                    ApprenticeshipCourse = _expectedApprenticeshipCourse,
                    Restriction = 1
                },
                new Rule
                {
                    ApprenticeshipId = 1,
                    ApprenticeshipCourse = _expectedApprenticeshipCourse,
                    Restriction = 2
                }
            });
            _reservation = new Reservation(_ruleRepository.Object);
            
        }

        [Test]
        public async Task Then_The_Rules_For_That_Reservation_Period_Are_Taken_From_The_Repository_And_Filtered_By_Account_Type()
        {
            //Arrange
            _reservation.IsLevyAccount = true;

            //Act
            await _reservation.GetReservationRules();

            //Assert
            _ruleRepository.Verify(x=>x.GetReservationRules(_reservation.CreatedDate, _reservation.ExpiryDate));
            Assert.AreEqual(2,_reservation.Rules.Count);
            Assert.IsTrue(_reservation.Rules.Any(c=>c.Restriction.Equals(AccountRestriction.All)));
            Assert.IsTrue(_reservation.Rules.Any(c=>c.Restriction.Equals(AccountRestriction.Levy)));
        }

        [Test]
        public void Then_The_Reservation_Is_Valid_If_It_Is_Within_The_Expiry_Period()
        {
            //Act
            _reservation.ExpiryDate = DateTime.UtcNow.AddDays(-1);

            //Assert
            Assert.IsTrue(_reservation.IsActive);
        }

        [Test]
        public void Then_The_Reservation_Is_Not_Valid_If_It_Has_Fallen_Out_Of_The_Expiry_Period()
        {
            //Act
            _reservation.ExpiryDate = DateTime.UtcNow.AddDays(1);

            //Assert
            Assert.IsFalse(_reservation.IsActive);
        }

        [Test]
        public async Task Then_The_Associated_Course_Is_Included()
        {
            //Arrange
            _reservation.IsLevyAccount = true;

            //Act
            await _reservation.GetReservationRules();

            //Assert
            Assert.AreEqual(2, _reservation.Rules.Count);
            Assert.IsTrue(_reservation.Rules.All(c=>c.ApprenticeshipCourse!=null));
            var actualRuleCourse = _reservation.Rules.FirstOrDefault()?.ApprenticeshipCourse;
            Assert.IsNotNull(actualRuleCourse);
            Assert.AreEqual(_expectedApprenticeshipCourse.Id, actualRuleCourse.Id);
            Assert.AreEqual(_expectedApprenticeshipCourse.CourseId, actualRuleCourse.CourseId);
            Assert.AreEqual(_expectedApprenticeshipCourse.Title, actualRuleCourse.Title);
            Assert.AreEqual(_expectedApprenticeshipCourse.Level.ToString(), actualRuleCourse.Level);
        }
    }
}
