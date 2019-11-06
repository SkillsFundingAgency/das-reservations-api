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
            _reservation = CreateReservation();
            
            //Assert
            Assert.AreEqual(2, _reservation.Rules.Count);
            Assert.IsTrue(_reservation.Rules.Any(c => c.Restriction.Equals(AccountRestriction.All)));
            Assert.IsTrue(_reservation.Rules.Any(c => c.Restriction.Equals(AccountRestriction.Levy)));
        }

        [Test]
        public void Then_The_Reservation_Is_Expired_If_It_Is_Within_The_Expiry_Period_And_Is_Pending()
        {
            //Act
            _reservation = CreateReservation();

            //Assert
            Assert.IsTrue(_reservation.IsExpired);
        }

        [Test]
        public void Then_The_Reservation_Is_Not_Expiring_If_It_Is_Within_The_Expiry_Period_But_Not_Pending()
        {
            //Act
            _reservation = new Reservation(null, Guid.NewGuid(), 1, false, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow.AddDays(-1), ReservationStatus.Confirmed, new Course(),0,0,"",0,null);


            //Assert
            Assert.IsFalse(_reservation.IsExpired);
        }

        [Test]
        public void Then_The_Reservation_Is_Expired_If_It_Is_Not_Within_The_Expiry_Period_And_Is_Pending()
        {
            //Act
            _reservation = new Reservation(null, Guid.NewGuid(), 1, false, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), ReservationStatus.Pending, new Course(),0,0,"",0, null);

            //Assert
            Assert.IsFalse(_reservation.IsExpired);
        }

        [Test]
        public void Then_The_Associated_Course_Is_Included()
        {
            //Act
            _reservation = CreateReservation();

            //Assert
            Assert.AreEqual(2, _reservation.Rules.Count);
            Assert.IsTrue(_reservation.Rules.All(c => c.Course != null));
            var actualRuleCourse = _reservation.Rules.FirstOrDefault()?.Course;
            Assert.IsNotNull(actualRuleCourse);
            Assert.AreEqual(_expectedCourse.CourseId, actualRuleCourse.CourseId);
            Assert.AreEqual(_expectedCourse.Title, actualRuleCourse.Title);
            Assert.AreEqual(_expectedCourse.Level.ToString(), actualRuleCourse.Level);
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
            Assert.AreEqual(expectedExpiryDate,_reservation.ExpiryDate);
        }

        [Test]
        public void Then_When_A_New_Reservation_It_Is_Initalised_Correctly()
        {
            //Arrange
            var expectedId = Guid.NewGuid();
            var expectedAccountId = 123;
            var expectedStartDate = DateTime.UtcNow;
            var expectedCourseId = "1-345-1";
            var expiryPeriodInMonths = 2;
            var expectedProviderId = 443322u;
            var expectedLegalEntityAccountId = 339988;
            var expectedLegalEntityAccountName = "TestName";
            var expectedTransferSenderId = 48752;

            //Act
            _reservation = new Reservation(expectedId,expectedAccountId, expectedStartDate,expiryPeriodInMonths, expectedLegalEntityAccountName, expectedCourseId, expectedProviderId, expectedLegalEntityAccountId,transferSenderAccountId:expectedTransferSenderId);

            //Assert
            Assert.AreEqual(expectedId, _reservation.Id);
            Assert.AreEqual(expectedAccountId, _reservation.AccountId);
            Assert.AreEqual(expectedStartDate, _reservation.StartDate);
            Assert.AreEqual(expectedCourseId, _reservation.CourseId);
            Assert.AreEqual(expectedProviderId, _reservation.ProviderId);
            Assert.AreEqual(expectedLegalEntityAccountId, _reservation.AccountLegalEntityId);
            Assert.AreEqual(expectedLegalEntityAccountName, _reservation.AccountLegalEntityName);
            Assert.AreEqual(expectedTransferSenderId, _reservation.TransferSenderAccountId);

        }

        [Test]
        public void Then_When_A_New_Reservation_Is_Created_Default_Values_Are_Used_If_Not_Provided()
        {
            //Arrange
            var expectedId = Guid.NewGuid();
            var expectedAccountId = 123;
            var expectedStartDate = DateTime.UtcNow;
            var expiryPeriodInMonths = 2;
            var expectedAccountLegalEntityId = 2524;
            var expecteLegalEntityAccountName = "TestName";

            //Act
            _reservation = new Reservation(expectedId, expectedAccountId, expectedStartDate, expiryPeriodInMonths,expecteLegalEntityAccountName,accountLegalEntityId: expectedAccountLegalEntityId);

            //Assert
            Assert.AreEqual(expectedId, _reservation.Id);
            Assert.AreEqual(expectedAccountId, _reservation.AccountId);
            Assert.AreEqual(expectedStartDate, _reservation.StartDate);
            Assert.AreEqual(expecteLegalEntityAccountName, _reservation.AccountLegalEntityName);
            Assert.AreEqual(expectedAccountLegalEntityId, _reservation.AccountLegalEntityId);
            Assert.IsNull(_reservation.CourseId);
            Assert.IsNull(_reservation.ProviderId);
        }

        [Test]
        public void Then_When_There_Is_No_StartDate_The_Expiry_Date_Is_Not_Set_And_No_Rules_Are_Retrieved()
        {
            //Arrange
            var expectedId = Guid.NewGuid();
            var expectedAccountId = 123;
            var expiryPeriodInMonths = 2;
            var expectedAccountLegalEntityId = 2524;
            var expecteLegalEntityAccountName = "TestName";

            //Act
            _reservation = new Reservation(expectedId, expectedAccountId, null, expiryPeriodInMonths, expecteLegalEntityAccountName, accountLegalEntityId: expectedAccountLegalEntityId);

            //Assert
            Assert.IsNull(_reservation.ExpiryDate);
            Assert.IsNull(_reservation.StartDate);
        }

        [Test]
        public void Then_When_There_Is_No_Start_Date_The_Rules_Are_Not_Returned()
        {
            //Act
            var reservation = new Reservation(_rules, Guid.NewGuid(), 1, true, DateTime.UtcNow, null, null, ReservationStatus.Pending, new Course(), 0, 0, "TestName",32, null);

            //Act
            Assert.IsEmpty(reservation.Rules);
        }

        private Reservation CreateReservation()
        {
            return new Reservation(_rules, Guid.NewGuid(), 1, true, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow.AddDays(-1), ReservationStatus.Pending, new Course(),0,0, "TestName",0, null);
        }
    }
}
