﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Queries
{
    public class WhenFindingReservations
    {
        private const long ExpectedAccountId = 553234;
        private const string ExpectedSearchTerm = "test";
        private const ushort ExpectedPageNumber = 2;
        private const ushort ExpectedPageItemCount = 50;
        private const ushort ExpectedSearchResultTotal = 1;


        private FindAccountReservationsQueryHandler _handler;
        private Mock<IValidator<FindAccountReservationsQuery>> _validator;
        private CancellationToken _cancellationToken;
        private FindAccountReservationsQuery _query;
        private Mock<IAccountReservationService> _service;
        private readonly List<Reservation> _expectedSearchResults = new List<Reservation>
        {
            new Reservation(Guid.NewGuid(), ExpectedAccountId, DateTime.Now, 3, "Test Name")
        };
        private readonly List<string> _expectedCourseFilters = new List<string>{"Baker - Level 1", "Banking - Level 3"};
        private SelectedSearchFilters _expectedSelectedFilters = new SelectedSearchFilters
        {
            CourseFilter = "Baker - Level 1"
        };

        [SetUp]
        public void Arrange()
        {
            _query = new FindAccountReservationsQuery
            {
                ProviderId = ExpectedAccountId, 
                SearchTerm = ExpectedSearchTerm,
                PageNumber = ExpectedPageNumber,
                PageItemCount = ExpectedPageItemCount,
                SelectedFilters = _expectedSelectedFilters
            };
            _validator = new Mock<IValidator<FindAccountReservationsQuery>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<FindAccountReservationsQuery>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });
            _cancellationToken = new CancellationToken();
            _service = new Mock<IAccountReservationService>();

            _service.Setup(x => x.FindReservations(
                    ExpectedAccountId, ExpectedSearchTerm, ExpectedPageNumber, 
                    ExpectedPageItemCount, It.IsAny<SelectedSearchFilters>()))
                .ReturnsAsync(new ReservationSearchResult
                {
                    Reservations = _expectedSearchResults,
                    TotalReservations = ExpectedSearchResultTotal,
                    Filters = new SearchFilters { CourseFilters = _expectedCourseFilters}
                });
            
            _handler = new FindAccountReservationsQueryHandler(_service.Object, _validator.Object);
        }

        [Test]
        public async Task Then_The_Query_Is_Validated()
        {
            //Act
            await _handler.Handle(_query, _cancellationToken);

            //Assert
            _validator.Verify(x=>x.ValidateAsync(_query), Times.Once);
        }

        [Test]
        public void Then_If_The_Query_Fails_Validation_Then_An_Argument_Exception_Is_Thrown()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<FindAccountReservationsQuery>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>{{"",""}} });

            //Act Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(_query, _cancellationToken));
        }

        [Test]
        public async Task Then_The_Service_Is_Called()
        {
            //Act
            await _handler.Handle(_query, _cancellationToken);

            //Assert
            _service.Verify(x => x.FindReservations(
                ExpectedAccountId, 
                ExpectedSearchTerm, 
                ExpectedPageNumber, 
                ExpectedPageItemCount,
                _expectedSelectedFilters), Times.Once);
        }

        [Test]
        public async Task Then_The_Service_Response_Is_Returned()
        {
            //Act
            var result = await _handler.Handle(_query, _cancellationToken);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_expectedSearchResults, result.Reservations);
            Assert.AreEqual(ExpectedSearchResultTotal, result.NumberOfRecordsFound);
            Assert.AreEqual(_expectedCourseFilters, result.Filters.CourseFilters);
        }
    }
}
