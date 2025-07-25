using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Services;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
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
        private Reservation _secondReservation;

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
                IsLevyAccount = false,
                Status = 2,
                CourseId = "123-1",
                Course = new Course
                {
                    CourseId = "123-1",
                    Level = 1,
                    Title = "Course 123-1"
                }
            };
            _secondReservation = new Reservation
            {
                AccountId = ExpectedAccountId,
                ExpiryDate = DateTime.Today.AddDays(45),
                StartDate = DateTime.Today.AddDays(15),
                CreatedDate = DateTime.Today,
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
                .ReturnsAsync(new List<Reservation> {_expectedReservation, _secondReservation });
            _service = new AccountReservationService(
                _reservationRepository.Object, 
                _ruleRepository.Object, 
                Mock.Of<IOptions<ReservationsConfiguration>>(), 
                Mock.Of<IAzureSearchReservationIndexRepository>(),
                Mock.Of<IAccountLegalEntitiesRepository>());
        }

        [Test]
        public async Task Then_The_Reservations_Are_Taken_From_The_Repository_For_That_Account()
        {
            //Act
            var actual = await _service.GetAccountReservations(ExpectedAccountId);

            //Assert
            _reservationRepository.Verify(x=>x.GetAccountReservations(ExpectedAccountId));
            actual.Should().NotBeNull();
        }

        [Test]
        public async Task Then_The_Reservations_Marked_As_Being_From_A_Levy_Account_Are_Not_Returned()
        {
            //Act
            var actual = await _service.GetAccountReservations(ExpectedAccountId);

            //Assert
            actual.Should().HaveCount(1);
        }

        [Test]
        public async Task Then_The_Results_Are_Mapped_To_The_Domain_Model()
        {
            //Act
            var actual = await _service.GetAccountReservations(ExpectedAccountId);

            //Act
            var actualReservation = actual.FirstOrDefault();
            actualReservation.Should().NotBeNull();
            actualReservation.Id.Should().Be(_expectedReservation.Id);
            actualReservation.AccountId.Should().Be(_expectedReservation.AccountId);
            actualReservation.StartDate.Should().Be(_expectedReservation.StartDate);
            actualReservation.ExpiryDate.Should().Be(_expectedReservation.ExpiryDate);
            actualReservation.CreatedDate.Should().Be(_expectedReservation.CreatedDate);
            actualReservation.IsLevyAccount.Should().Be(_expectedReservation.IsLevyAccount);
            actualReservation.Status.Should().Be((ReservationStatus)_expectedReservation.Status);
            actualReservation.Course.CourseId.Should().Be(_expectedReservation.Course.CourseId);
            actualReservation.Course.Title.Should().Be(_expectedReservation.Course.Title);
            actualReservation.Course.Level.Should().Be(_expectedReservation.Course.Level.ToString());

        }
    }
}
