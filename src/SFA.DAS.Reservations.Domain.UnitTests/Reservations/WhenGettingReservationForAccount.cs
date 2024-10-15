using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
            _reservation = CreateReservation();
            
            //Assert
            _reservation.Rules.Count.Should().Be(2);
            _reservation.Rules.Should().Contain(c => c.Restriction.Equals(AccountRestriction.All));
            _reservation.Rules.Should().Contain(c => c.Restriction.Equals(AccountRestriction.Levy));
        }

        [Test]
        public void Then_The_Reservation_Is_Expired_If_It_Is_Within_The_Expiry_Period_And_Is_Pending()
        {
            //Act
            _reservation = CreateReservation();

            //Assert
            _reservation.IsExpired.Should().BeTrue();
        }

        [Test]
        public void Then_The_Reservation_Is_Not_Expiring_If_It_Is_Within_The_Expiry_Period_But_Not_Pending()
        {
            //Act
            _reservation = new Reservation(null, Guid.NewGuid(), 1, false, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow.AddDays(-1), ReservationStatus.Confirmed, new Course(),0,0,"",0,null);


            //Assert
            _reservation.IsExpired.Should().BeFalse();
        }

        [Test]
        public void Then_The_Reservation_Is_Expired_If_It_Is_Not_Within_The_Expiry_Period_And_Is_Pending()
        {
            //Act
            _reservation = new Reservation(null, Guid.NewGuid(), 1, false, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), ReservationStatus.Pending, new Course(),0,0,"",0, null);

            //Assert
            _reservation.IsExpired.Should().BeFalse();
        }

        [Test]
        public void Then_The_Associated_Course_Is_Included()
        {
            //Act
            _reservation = CreateReservation();

            //Assert
            _reservation.Rules.Count.Should().Be(2);
            _reservation.Rules.All(c => c.Course != null).Should().BeTrue();
            var actualRuleCourse = _reservation.Rules.FirstOrDefault()?.Course;
            actualRuleCourse.Should().NotBeNull();
            actualRuleCourse.Should().BeEquivalentTo(new
            {
                _expectedCourse.CourseId,
                _expectedCourse.Title,
                Level = _expectedCourse.Level.ToString()
            });
        }

        [TestCase(2018,04,30)]
        [TestCase(2018,02,28)]
        [TestCase(2018,10,31)]
        public void Then_When_Creating_A_New_Reservation_The_Expiry_Is_Set_To_The_End_Of_The_Last_Day_Of_The_Month_With_Added_Expiry_Months(int year, int month, int day)
        {
            //Arrange
            var expectedExpiryDate = new DateTime(year, month, day, 23, 59, 59);
            var expiryPeriod = 1;

            //Act
            _reservation = new Reservation(Guid.NewGuid(), 123, new DateTime(2018,month-expiryPeriod,03),expiryPeriod,"TestName");

            //Assert
            _reservation.ExpiryDate.Should().Be(expectedExpiryDate);
        }

        [Test]
        public void Then_When_A_New_Reservation_It_Is_Initalised_Correctly()
        {
            //Arrange
            var expectedId = Guid.NewGuid();
            var expectedAccountId = 123;
            var expectedStartDate = DateTime.UtcNow;
            const string expectedCourseId = "1-345-1";
            const int expiryPeriodInMonths = 2;
            const uint expectedProviderId = 443322u;
            const int expectedLegalEntityAccountId = 339988;
            const string expectedLegalEntityAccountName = "TestName";
            const int expectedTransferSenderId = 48752;

            //Act
            _reservation = new Reservation(expectedId,expectedAccountId, expectedStartDate,expiryPeriodInMonths, expectedLegalEntityAccountName, expectedCourseId, expectedProviderId, expectedLegalEntityAccountId,transferSenderAccountId:expectedTransferSenderId);

            //Assert
            _reservation.Id.Should().Be(expectedId);
            _reservation.AccountId.Should().Be(expectedAccountId);
            _reservation.StartDate.Should().Be(expectedStartDate);
            _reservation.CourseId.Should().Be(expectedCourseId);
            _reservation.ProviderId.Should().Be(expectedProviderId);
            _reservation.AccountLegalEntityId.Should().Be(expectedLegalEntityAccountId);
            _reservation.AccountLegalEntityName.Should().Be(expectedLegalEntityAccountName);
            _reservation.TransferSenderAccountId.Should().Be(expectedTransferSenderId);
        }

        [Test]
        public void Then_When_A_New_Reservation_Is_Created_Default_Values_Are_Used_If_Not_Provided()
        {
            //Arrange
            var expectedId = Guid.NewGuid();
            const int expectedAccountId = 123;
            var expectedStartDate = DateTime.UtcNow;
            const int expiryPeriodInMonths = 2;
            const int expectedAccountLegalEntityId = 2524;
            const string expecteLegalEntityAccountName = "TestName";

            //Act
            _reservation = new Reservation(expectedId, expectedAccountId, expectedStartDate, expiryPeriodInMonths,expecteLegalEntityAccountName,accountLegalEntityId: expectedAccountLegalEntityId);

            //Assert
            _reservation.Id.Should().Be(expectedId);
            _reservation.AccountId.Should().Be(expectedAccountId);
            _reservation.StartDate.Should().Be(expectedStartDate);
            _reservation.AccountLegalEntityName.Should().Be(expecteLegalEntityAccountName);
            _reservation.AccountLegalEntityId.Should().Be(expectedAccountLegalEntityId);
            _reservation.CourseId.Should().NotBeNull();
            _reservation.ProviderId.Should().NotBeNull();
        }

        [Test]
        public void Then_When_There_Is_No_StartDate_The_Expiry_Date_Is_Not_Set_And_No_Rules_Are_Retrieved()
        {
            //Arrange
            var expectedId = Guid.NewGuid();
            const int expectedAccountId = 123;
            const int expiryPeriodInMonths = 2;
            const int expectedAccountLegalEntityId = 2524;
            const string expecteLegalEntityAccountName = "TestName";

            //Act
            _reservation = new Reservation(expectedId, expectedAccountId, null, expiryPeriodInMonths, expecteLegalEntityAccountName, accountLegalEntityId: expectedAccountLegalEntityId);

            //Assert
            _reservation.ExpiryDate.Should().BeNull();
            _reservation.StartDate.Should().BeNull();
        }

        [Test]
        public void Then_When_There_Is_No_Start_Date_The_Rules_Are_Not_Returned()
        {
            //Act
            var reservation = new Reservation(_rules, Guid.NewGuid(), 1, true, DateTime.UtcNow, null, null, ReservationStatus.Pending, new Course(), 0, 0, "TestName",32, null);

            //Act
            reservation.Rules.Should().BeEmpty();
        }

        private Reservation CreateReservation()
        {
            return new Reservation(_rules, Guid.NewGuid(), 1, true, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow.AddDays(-1), ReservationStatus.Pending, new Course(),0,0, "TestName",0, null);
        }
    }
}
