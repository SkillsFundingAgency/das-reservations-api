﻿using System;
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
        private readonly List<string> _expectedCourseFilters = new List<string> { "Baker - Level 1", "Banking - Level 3" };
        private SearchFilters _expectedSearchFilter;

        [SetUp]
        public void Init()
        {
            _reservationRepository = new Mock<IReservationRepository>();
            _reservationIndexRepository = new Mock<IReservationIndexRepository>();
            _ruleRepository = new Mock<IRuleRepository>();
            _options = new Mock<IOptions<ReservationsConfiguration>>();

            _service = new AccountReservationService(_reservationRepository.Object, _ruleRepository.Object,
                _options.Object, _reservationIndexRepository.Object);

            _expectedSearchFilter = new SearchFilters
            {
                CourseFilters = _expectedCourseFilters
            };

            _reservationIndexRepository.Setup(x => x.Find(
                    ProviderId, SearchTerm, PageNumber, PageItemNumber, It.IsAny<SearchFilters>()))
                .ReturnsAsync(new IndexedReservationSearchResult());
        }

        [Test]
        public async Task ThenShouldSearchRepository()
        {
            //Act
            await _service.FindReservations(ProviderId, SearchTerm, PageNumber, PageItemNumber,
                                            _expectedSearchFilter);

            //Assert
            _reservationIndexRepository.Verify(x => x.Find(
                ProviderId, SearchTerm, PageNumber, PageItemNumber,
                It.Is<SearchFilters>(sf => sf.CourseFilters.Equals(_expectedCourseFilters))), Times.Once);
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
                    ProviderId, SearchTerm, PageNumber, PageItemNumber, It.IsAny<SearchFilters>()))
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
            var result = await _service.FindReservations(ProviderId, SearchTerm, PageNumber, PageItemNumber,
                                                         _expectedSearchFilter);

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
                    ProviderId, SearchTerm, PageNumber, PageItemNumber, It.IsAny<SearchFilters>()))
                .ReturnsAsync(new IndexedReservationSearchResult
                {
                    Reservations = new List<ReservationIndex>(),
                    TotalReservations = 0,
                    Filters = new SearchFilters { CourseFilters = expectedFilters}
                });

            //Act
            var result = await _service.FindReservations(ProviderId, SearchTerm, PageNumber, PageItemNumber,
                                                        _expectedSearchFilter);

            //Assert
            result.Filters.CourseFilters.Should().BeEquivalentTo(expectedFilters);
        }

        [Test]
        public async Task ThenShouldReturnNoReservationsIfNoneFound()
        {
            //Arrange
            var expectedReservation = new Reservation(
                Guid.NewGuid(), 1, DateTime.Now, 3,
                "Test Reservation", null, 3, 4);

            //Act
            var result = await _service.FindReservations(ProviderId, SearchTerm, PageNumber, PageItemNumber,
                                                         _expectedSearchFilter);

            //Assert
            result.Reservations.Should().BeEmpty();
            result.TotalReservations.Should().Be(0);
        }
    }
}
