using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Services;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Services
{
    public class WhenCreatingAccountReservations
    {
        private Mock<IRuleRepository> _ruleRepository;
        private Mock<IReservationRepository> _reservationRepository;
        private AccountReservationService _accountReservationService;

        private const int ExpiryPeriodInMonths = 5;
        private const long ExpectedAccountId = 12344;
        private const long ExpectedReservationId = 55323;
        private readonly DateTime _expectedStartDate = DateTime.UtcNow.AddMonths(1);
        private Mock<IOptions<ReservationConfiguration>> _options;

        [SetUp]
        public void Arrange()
        {
            _options = new Mock<IOptions<ReservationConfiguration>>();
            _options.Setup(x => x.Value.ExpiryPeriodInMonths).Returns(ExpiryPeriodInMonths);
            _reservationRepository = new Mock<IReservationRepository>();
            _ruleRepository = new Mock<IRuleRepository>();

            _reservationRepository.Setup(x => x.CreateAccountReservation(ExpectedAccountId, _expectedStartDate))
                .ReturnsAsync(ExpectedReservationId);

            _accountReservationService = new AccountReservationService(_reservationRepository.Object, _ruleRepository.Object);
        }

        [Test]
        public async Task Then_The_Expiry_Period_For_The_Reservation_Is_Taken_From_Configuration()
        {
            //Act
            await _accountReservationService.CreateAccountReservation(ExpectedAccountId, _expectedStartDate);

            //Assert

        }

        [Test]
        public async Task Then_The_Repository_Is_Called_To_Create_A_Reservation_Mapping_To_The_Entity()
        {
            //Act
            await _accountReservationService.CreateAccountReservation(ExpectedAccountId, _expectedStartDate);

            //Assert
            _reservationRepository.Verify(x=>x.CreateAccountReservation(ExpectedAccountId,_expectedStartDate));
        }

        [Test]
        public async Task Then_The_ReservationId_Is_Returned()
        {
            //Act
            var actual = await _accountReservationService.CreateAccountReservation(ExpectedAccountId, _expectedStartDate);

            //Assert
            Assert.AreEqual(ExpectedReservationId, actual);
        }

    }
}
