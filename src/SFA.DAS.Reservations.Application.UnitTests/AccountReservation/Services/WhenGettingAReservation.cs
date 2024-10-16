using System;
using System.Collections.Generic;
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
    public class WhenGettingAReservation
    {
        private AccountReservationService _service;
        private Mock<IReservationRepository> _reservationRepository;
        private Mock<IRuleRepository> _ruleRepository;
        private Domain.Entities.Reservation _expectedReservation;

        private readonly Guid _expectedReservationId = Guid.NewGuid();
        [SetUp]
        public void Arrange()
        {
            _ruleRepository = new Mock<IRuleRepository>();
            _ruleRepository.Setup(x => x.GetReservationRules(It.IsAny<DateTime>())).ReturnsAsync(new List<Rule>());
            _reservationRepository = new Mock<IReservationRepository>();
            _expectedReservation = new Reservation
            {
                AccountId = 456789,
                ExpiryDate = DateTime.Today.AddDays(45),
                StartDate = DateTime.Today.AddDays(15),
                CreatedDate = DateTime.Today,
                Id = _expectedReservationId,
                IsLevyAccount = true,
                Status = 2,
                AccountLegalEntityName = "TestName",
                TransferSenderAccountId = 786876,
                UserId = Guid.NewGuid()
            };
            _reservationRepository.Setup(x => x.GetById(_expectedReservationId))
                .ReturnsAsync(_expectedReservation);
            _service = new AccountReservationService(
                _reservationRepository.Object, 
                _ruleRepository.Object, 
                Mock.Of<IOptions<ReservationsConfiguration>>(), 
                Mock.Of<IReservationIndexRepository>(),
                Mock.Of<IAccountLegalEntitiesRepository>());
        }

        [Test]
        public async Task Then_The_Reservations_Are_Taken_From_The_Repository_For_That_Account()
        {
            //Act
            var actual = await _service.GetReservation(_expectedReservationId);

            //Assert
            _reservationRepository.Verify(x => x.GetById(_expectedReservationId));
            actual.Should().NotBeNull();
        }

        [Test]
        public async Task Then_The_Results_Are_Mapped_To_The_Domain_Model()
        {
            //Act
            var actual = await _service.GetReservation(_expectedReservationId);

            //Act
            actual.Should().NotBeNull();
            actual.Id.Should().Be(_expectedReservation.Id);
            actual.AccountId.Should().Be(_expectedReservation.AccountId);
            actual.StartDate.Should().Be(_expectedReservation.StartDate);
            actual.ExpiryDate.Should().Be(_expectedReservation.ExpiryDate);
            actual.CreatedDate.Should().Be(_expectedReservation.CreatedDate);
            actual.IsLevyAccount.Should().Be(_expectedReservation.IsLevyAccount);
            actual.Status.Should().Be((ReservationStatus)_expectedReservation.Status);
            actual.AccountLegalEntityName.Should().Be(_expectedReservation.AccountLegalEntityName);
            actual.TransferSenderAccountId.Should().Be(_expectedReservation.TransferSenderAccountId);
            actual.UserId.Should().Be(_expectedReservation.UserId);
        }

        [Test]
        public async Task Then_If_The_Repository_Returns_Null_Then_Null_Is_Returned()
        {
            //Act
            var actual = await _service.GetReservation(Guid.NewGuid());

            //Assert
            actual.Should().BeNull();
        }
    }
}
