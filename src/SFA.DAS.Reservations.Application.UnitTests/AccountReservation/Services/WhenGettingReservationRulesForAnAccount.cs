using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Services;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Services
{
    public class WhenGettingReservationRulesForAnAccount
    {
        private AccountReservationService _service;
        private Mock<IReservationRepository> _reservationRepository;
        private Mock<IRuleRepository> _ruleRepository;
        private Reservation _expectedReservation;

        private const long ExpectedAccountId = 66532;

        [SetUp]
        public void Arrange()
        {
            _ruleRepository = new Mock<IRuleRepository>();
            _ruleRepository.Setup(x => x.GetReservationRules(It.IsAny<DateTime>())).ReturnsAsync(new List<Rule>());
            _reservationRepository = new Mock<IReservationRepository>();
            _expectedReservation = new Reservation
            {
                AccountId = ExpectedAccountId,
                ExpiryDate = DateTime.Today.AddDays(45),
                StartDate = DateTime.Today.AddDays(15),
                CreatedDate= DateTime.Today,
                Id = Guid.NewGuid(),
                IsLevyAccount = true,
                Status = 2,
                CourseId = "123-1",
                Course = new Course
                {
                    CourseId = "123-1",
                    Level = 1,
                    Title = "Course 123-1"
                }
            };
            _reservationRepository.Setup(x => x.GetAccountReservations(ExpectedAccountId))
                .ReturnsAsync(new List<Reservation> {_expectedReservation});
            _service = new AccountReservationService(_reservationRepository.Object, _ruleRepository.Object, Mock.Of<IOptions<ReservationsConfiguration>>());
        }

        [Test]
        public async Task Then_The_Reservations_Are_Taken_From_The_Repository_For_That_Account()
        {
            //Act
            var actual = await _service.GetAccountReservations(ExpectedAccountId);

            //Assert
            _reservationRepository.Verify(x=>x.GetAccountReservations(ExpectedAccountId));
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task Then_The_Results_Are_Mapped_To_The_Domain_Model()
        {
            //Act
            var actual = await _service.GetAccountReservations(ExpectedAccountId);

            //Act
            var actualReservation = actual.FirstOrDefault();
            Assert.IsNotNull(actualReservation);
            Assert.AreEqual(_expectedReservation.Id, actualReservation.Id);
            Assert.AreEqual(_expectedReservation.AccountId, actualReservation.AccountId);
            Assert.AreEqual(_expectedReservation.StartDate, actualReservation.StartDate);
            Assert.AreEqual(_expectedReservation.ExpiryDate, actualReservation.ExpiryDate);
            Assert.AreEqual(_expectedReservation.CreatedDate, actualReservation.CreatedDate);
            Assert.AreEqual(_expectedReservation.IsLevyAccount, actualReservation.IsLevyAccount);
            Assert.AreEqual(_expectedReservation.Status, (short)actualReservation.Status);
            Assert.AreEqual(_expectedReservation.Course.CourseId, actualReservation.Course.CourseId);
            Assert.AreEqual(_expectedReservation.Course.Title, actualReservation.Course.Title);
            Assert.AreEqual(_expectedReservation.Course.Level.ToString(), actualReservation.Course.Level);
        }
    }
}
