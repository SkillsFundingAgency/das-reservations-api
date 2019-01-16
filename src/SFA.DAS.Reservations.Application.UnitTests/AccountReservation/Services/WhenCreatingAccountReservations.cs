using System;
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
        private Reservation _reservation;

        [SetUp]
        public void Arrange()
        {
            _options = new Mock<IOptions<ReservationConfiguration>>();
            _options.Setup(x => x.Value.ExpiryPeriodInMonths).Returns(ExpiryPeriodInMonths);
            _reservationRepository = new Mock<IReservationRepository>();
            _ruleRepository = new Mock<IRuleRepository>();

            _reservationRepository.Setup(x => x.CreateAccountReservation(It.Is<Domain.Entities.Reservation>(c=>c.AccountId.Equals(ExpectedAccountId))))
                .ReturnsAsync(ExpectedReservationId);

            _reservation = new Reservation(_ruleRepository.Object)
            {
                AccountId = ExpectedAccountId,
                StartDate = _expectedStartDate
            };

            _accountReservationService = new AccountReservationService(_reservationRepository.Object, _ruleRepository.Object, _options.Object);
        }

        [Test]
        public async Task Then_The_Expiry_Period_For_The_Reservation_Is_Taken_From_Configuration()
        {
            //Act
            await _accountReservationService.CreateAccountReservation(_reservation);

            //Assert
            _options.Verify(x=>x.Value.ExpiryPeriodInMonths,Times.Once);
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_To_Create_A_Reservation_Mapping_To_The_Entity()
        {
            //Act
            await _accountReservationService.CreateAccountReservation(_reservation);

            //Assert
            _reservationRepository.Verify(x=>x.CreateAccountReservation(It.Is<Domain.Entities.Reservation>(
                c=>c.AccountId.Equals(ExpectedAccountId) &&
                   c.StartDate.Equals(_expectedStartDate) &&
                   c.ExpiryDate.Equals(_expectedStartDate.AddMonths(ExpiryPeriodInMonths))
                )));
        }

        [Test]
        public async Task Then_The_ReservationId_Is_Returned()
        {
            //Act
            var actual = await _accountReservationService.CreateAccountReservation(_reservation);

            //Assert
            Assert.AreEqual(ExpectedReservationId, actual);
        }

    }
}
