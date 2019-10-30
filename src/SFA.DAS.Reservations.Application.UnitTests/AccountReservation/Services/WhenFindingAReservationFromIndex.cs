using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Services;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Services
{
    public class WhenFindingAReservationFromIndex
    {
        private AccountReservationService _service;
        private Mock<IReservationRepository> _reservationRepository;
        private Mock<IReservationIndexRepository> _reservationIndexRepository;
        private Mock<IRuleRepository> _ruleRepository;
        private Mock<IOptions<ReservationsConfiguration>> _options;

        [SetUp]
        public void Init()
        {
            _reservationRepository = new Mock<IReservationRepository>();
            _reservationIndexRepository = new Mock<IReservationIndexRepository>();
            _ruleRepository = new Mock<IRuleRepository>();
            _options = new Mock<IOptions<ReservationsConfiguration>>();

            _service = new AccountReservationService(_reservationRepository.Object, _ruleRepository.Object,
                _options.Object, _reservationIndexRepository.Object);
        }

        [Test]
        public async Task ThenShouldSearchRepository()
        {
            //Arrange
            const int providerId = 2;
            const string searchTerm = "test search";

            //Act
            await _service.FindReservations(providerId, searchTerm);

            //Assert
            _reservationIndexRepository.Verify(x => x.Find(providerId, searchTerm), Times.Once);
        }

        [Test]
        public async Task ThenShouldReturnReservationFound()
        {
            //Arrange
            const int providerId = 2;
            const string searchTerm = "test search";
            var expectedReservation = new Reservation(
                Guid.NewGuid(), 1, DateTime.Now, 3,
                "Test Reservation", null, 3, 4);


            _reservationIndexRepository.Setup(x => x.Find(providerId, searchTerm))
                .ReturnsAsync(new List<ReservationIndex>
                {
                    new ReservationIndex()
                    {
                        AccountId = expectedReservation.AccountId,
                        AccountLegalEntityId = expectedReservation.AccountLegalEntityId,
                        ProviderId = expectedReservation.ProviderId,
                        StartDate = expectedReservation.StartDate,
                        ExpiryDate = expectedReservation.ExpiryDate,
                        IsLevyAccount = expectedReservation.IsLevyAccount,
                        Status = (short) expectedReservation.Status,
                        ReservationId = expectedReservation.Id,
                        CreatedDate = expectedReservation.CreatedDate,
                        AccountLegalEntityName = expectedReservation.AccountLegalEntityName
                    }
                });

            //Act
            var result = await _service.FindReservations(providerId, searchTerm);

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.First().Should().BeEquivalentTo(expectedReservation);
        }
    }
}
