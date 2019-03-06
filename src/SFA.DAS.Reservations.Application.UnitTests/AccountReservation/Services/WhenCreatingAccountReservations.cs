using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Services;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using Reservation = SFA.DAS.Reservations.Domain.Reservations.Reservation;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Services
{
    public class WhenCreatingAccountReservations
    {
        private Mock<IRuleRepository> _ruleRepository;
        private Mock<IReservationRepository> _reservationRepository;
        private AccountReservationService _accountReservationService;

        private const int ExpiryPeriodInMonths = 5;
        private const long ExpectedAccountId = 12344;
        private Course _expectedCourse;
        private readonly Guid _expectedReservationId = Guid.NewGuid();
        private readonly DateTime _expectedStartDate = DateTime.UtcNow.AddMonths(1);
        private Mock<IOptions<ReservationsConfiguration>> _options;
        
        [SetUp]
        public void Arrange()
        {
            _expectedCourse = new Course
            {
                CourseId = "123-1",
                Title = "Course 123-1",
                Level = 1
            };

            _options = new Mock<IOptions<ReservationsConfiguration>>();
            _options.Setup(x => x.Value.ExpiryPeriodInMonths).Returns(ExpiryPeriodInMonths);
            _reservationRepository = new Mock<IReservationRepository>();
            _ruleRepository = new Mock<IRuleRepository>();
            _ruleRepository.Setup(x => x.GetReservationRules(It.IsAny<DateTime>())).ReturnsAsync(new List<Domain.Entities.Rule>());

            _reservationRepository.Setup(x => x.CreateAccountReservation(It.Is<Domain.Entities.Reservation>(c=>c.AccountId.Equals(ExpectedAccountId))))
                .ReturnsAsync(new Domain.Entities.Reservation{Id=_expectedReservationId, AccountId = ExpectedAccountId, Course = _expectedCourse});
            
            _accountReservationService = new AccountReservationService(_reservationRepository.Object, _ruleRepository.Object, _options.Object);
        }

        [Test]
        public async Task Then_The_Expiry_Period_For_The_Reservation_Is_Taken_From_Configuration()
        {
            //Act
            await _accountReservationService.CreateAccountReservation(ExpectedAccountId, _expectedStartDate);

            //Assert
            _options.Verify(x=>x.Value.ExpiryPeriodInMonths,Times.Once);
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_To_Create_A_Reservation_Mapping_To_The_Entity()
        {
            //Arrange
            var expiryDate = _expectedStartDate.AddMonths(ExpiryPeriodInMonths);
            var expectedExpiryDate = new DateTime(expiryDate.Year,expiryDate.Month, DateTime.DaysInMonth(expiryDate.Year, expiryDate.Month));

            //Act
            await _accountReservationService.CreateAccountReservation(ExpectedAccountId, _expectedStartDate);

            //Assert
            _reservationRepository.Verify(x=>x.CreateAccountReservation(It.Is<Domain.Entities.Reservation>(
                c=>c.AccountId.Equals(ExpectedAccountId) &&
                   c.StartDate.Equals(_expectedStartDate) &&
                   !c.CreatedDate.Equals(DateTime.MinValue) &&
                   c.ExpiryDate.Equals(expectedExpiryDate) &&
                   c.Status.Equals((short)ReservationStatus.Pending)
                )));
        }

        [Test]
        public async Task Then_The_New_Reservation_Is_Returned_Mapped_From_The_Entity()
        {
            //Act
            var actual = await _accountReservationService.CreateAccountReservation(ExpectedAccountId, _expectedStartDate);

            //Assert
            Assert.IsAssignableFrom<Reservation>(actual);
            Assert.AreEqual(_expectedReservationId, actual.Id);
            Assert.AreEqual(ExpectedAccountId, actual.AccountId);
            Assert.IsNotNull(actual.Rules);
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_To_Create_A_Reservation_With_Course_Mapping_To_The_Entity()
        {
            //Act
            await _accountReservationService.CreateAccountReservation(
                ExpectedAccountId, 
                _expectedStartDate, 
                _expectedCourse.CourseId);

            //Assert
            _reservationRepository.Verify(x=>x.CreateAccountReservation(It.Is<Domain.Entities.Reservation>(
                c=>c.AccountId.Equals(ExpectedAccountId) &&
                   c.StartDate.Equals(_expectedStartDate) &&
                   !c.CreatedDate.Equals(DateTime.MinValue) &&
                   c.ExpiryDate.Equals(_expectedStartDate.AddMonths(ExpiryPeriodInMonths)) &&
                   c.Status.Equals((short)ReservationStatus.Pending) &&
                   c.CourseId.Equals(_expectedCourse.CourseId) &&
                   c.Course == null
            )));
        }

        [Test]
        public async Task Then_The_New_Reservation_With_Course_Is_Returned_Mapped_From_The_Entity()
        {
            //Act
            var actual = await _accountReservationService.CreateAccountReservation(
                ExpectedAccountId, 
                _expectedStartDate, 
                _expectedCourse.CourseId);

            //Assert
            Assert.IsAssignableFrom<Reservation>(actual);
            Assert.AreEqual(_expectedReservationId, actual.Id);
            Assert.AreEqual(ExpectedAccountId, actual.AccountId);
            Assert.AreEqual(_expectedCourse.CourseId, actual.Course.Id);
            Assert.AreEqual(_expectedCourse.Title, actual.Course.Title);
            Assert.AreEqual(_expectedCourse.Level, actual.Course.Level);
            Assert.IsNotNull(actual.Rules);
        }

    }
}
