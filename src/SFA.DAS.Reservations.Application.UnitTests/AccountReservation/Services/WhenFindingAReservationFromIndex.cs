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
        private const int ProviderId = 2;
        private const string SearchTerm = "test search";
        private const ushort PageNumber = 2;
        private const ushort PageItemNumber = 2;

        private AccountReservationService _service;
        private Mock<IReservationRepository> _reservationRepository;
        private Mock<IReservationIndexRepository> _reservationIndexRepository;
        private Mock<IRuleRepository> _ruleRepository;
        private Mock<IOptions<ReservationsConfiguration>> _options;
        private SelectedSearchFilters _expectedSelectedFilter;

        [SetUp]
        public void Init()
        {
            _reservationRepository = new Mock<IReservationRepository>();
            _reservationIndexRepository = new Mock<IReservationIndexRepository>();
            _ruleRepository = new Mock<IRuleRepository>();
            _options = new Mock<IOptions<ReservationsConfiguration>>();

            _service = new AccountReservationService(_reservationRepository.Object, _ruleRepository.Object,
                _options.Object, _reservationIndexRepository.Object);

            _expectedSelectedFilter = new SelectedSearchFilters
            {
                CourseFilter = "Baker - Level 1",
                EmployerNameFilter = "Test Ltd",
                StartDateFilter = DateTime.Now.ToString("g")
            };

            _reservationIndexRepository.Setup(x => x.Find(
                    ProviderId, SearchTerm, PageNumber, PageItemNumber, It.IsAny<SelectedSearchFilters>()))
                .ReturnsAsync(new IndexedReservationSearchResult());
        }

        [Test]
        public async Task ThenShouldSearchRepository()
        {
            //Act
            await _service.FindReservations(ProviderId, SearchTerm, PageNumber, PageItemNumber, _expectedSelectedFilter);

            //Assert
            _reservationIndexRepository.Verify(x => x.Find(
                ProviderId, SearchTerm, PageNumber, PageItemNumber, _expectedSelectedFilter), Times.Once);
        }

        [Test]
        public async Task ThenShouldReturnReservationFound()
        {
            //Arrange
            var expectedReservation = new Reservation(
                Guid.NewGuid(), 1, DateTime.Now, 3,
                "Test Reservation", null, 3, 4);

            const int expectedSearchTotal = 1;

            _reservationIndexRepository.Setup(x => x.Find(
                    ProviderId, SearchTerm, PageNumber, PageItemNumber, _expectedSelectedFilter))
                .ReturnsAsync(new IndexedReservationSearchResult
                { 
                    Reservations = new List<ReservationIndex>
                    {
                        new ReservationIndex
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
                    },
                    TotalReservations = expectedSearchTotal
                });

            //Act
            var result = await _service.FindReservations(
                ProviderId, SearchTerm, PageNumber, PageItemNumber, _expectedSelectedFilter);

            //Assert
            result.Reservations.Should().NotBeNullOrEmpty();
            result.Reservations.First().Should().BeEquivalentTo(expectedReservation);
            result.TotalReservations.Should().Be(expectedSearchTotal);
        }

        [Test]
        public async Task ThenShouldReturnAvailableCourseFilters()
        {
            //Arrange
            var expectedFilters = new List<string>{"Test1", "Test2"};

            
            _reservationIndexRepository.Setup(x => x.Find(
                    ProviderId, SearchTerm, PageNumber, PageItemNumber, _expectedSelectedFilter))
                .ReturnsAsync(new IndexedReservationSearchResult
                {
                    Reservations = new List<ReservationIndex>(),
                    TotalReservations = 0,
                    Filters = new SearchFilters { CourseFilters = expectedFilters}
                });

            //Act
            var result = await _service.FindReservations(
                ProviderId, SearchTerm, PageNumber, PageItemNumber, _expectedSelectedFilter);

            //Assert
            result.Filters.CourseFilters.Should().BeEquivalentTo(expectedFilters);
        }

        [Test]
        public async Task ThenShouldReturnAvailableEmployerNameFilters()
        {
            //Arrange
            var expectedFilters = new List<string>{"Test1", "Test2"};

            
            _reservationIndexRepository.Setup(x => x.Find(
                    ProviderId, SearchTerm, PageNumber, PageItemNumber, _expectedSelectedFilter))
                .ReturnsAsync(new IndexedReservationSearchResult
                {
                    Reservations = new List<ReservationIndex>(),
                    TotalReservations = 0,
                    Filters = new SearchFilters { EmployerFilters = expectedFilters}
                });

            //Act
            var result = await _service.FindReservations(
                ProviderId, SearchTerm, PageNumber, PageItemNumber, _expectedSelectedFilter);

            //Assert
            result.Filters.EmployerFilters.Should().BeEquivalentTo(expectedFilters);
        }

        [Test]
        public async Task ThenShouldReturnAvailableStartDateFilters()
        {
            //Arrange
            var expectedFilters = new List<string>{DateTime.Now.AddDays(-1).ToString("g"), DateTime.Now.ToString("g")};
            
            _reservationIndexRepository.Setup(x => x.Find(
                    ProviderId, SearchTerm, PageNumber, PageItemNumber, _expectedSelectedFilter))
                .ReturnsAsync(new IndexedReservationSearchResult
                {
                    Reservations = new List<ReservationIndex>(),
                    TotalReservations = 0,
                    Filters = new SearchFilters { StartDateFilters = expectedFilters}
                });

            //Act
            var result = await _service.FindReservations(
                ProviderId, SearchTerm, PageNumber, PageItemNumber, _expectedSelectedFilter);

            //Assert
            result.Filters.StartDateFilters.Should().BeEquivalentTo(expectedFilters);
        }

        [Test]
        public async Task ThenShouldReturnNoReservationsIfNoneFound()
        {
            //Arrange
            var expectedReservation = new Reservation(
                Guid.NewGuid(), 1, DateTime.Now, 3,
                "Test Reservation", null, 3, 4);

            //Act
            var result = await _service.FindReservations(
                ProviderId, SearchTerm, PageNumber, PageItemNumber, _expectedSelectedFilter);

            //Assert
            result.Reservations.Should().BeEmpty();
            result.TotalReservations.Should().Be(0);
        }
    }
}
