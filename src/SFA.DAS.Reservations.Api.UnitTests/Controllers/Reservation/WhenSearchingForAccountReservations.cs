using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Reservation
{
    public class WhenSearchingForAccountReservations
    {
        private ReservationsController _reservationsController;
        private Mock<IMediator> _mediator;
        private FindAccountReservationsResult _accountReservationsResult;
        private const long ExpectedProviderId = 123234;
        private const string ExpectedSearchTerm = "test";
         
        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _accountReservationsResult = new FindAccountReservationsResult{Reservations= new List<Domain.Reservations.Reservation>
            {
                new Domain.Reservations.Reservation(Guid.NewGuid(), ExpectedProviderId, DateTime.Now, 3, "Test Name")
            },
                NumberOfRecordsFound = 3,
                Filters = new SearchFilters
                {
                    CourseFilters = new [] {"Baker - Level 1", "Banking - Level 2"},
                    EmployerFilters = new [] {"Test Ltd", "Acme Bank"},
                    StartDateFilters = new [] {DateTime.Now.AddDays(-1).ToString("d"), DateTime.Now.ToString("d")}
                }
            };
            
            _mediator.Setup(x => x.Send(It.Is<FindAccountReservationsQuery>(c => 
                        c.ProviderId.Equals(ExpectedProviderId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_accountReservationsResult);

            _reservationsController = new ReservationsController(Mock.Of<ILogger<ReservationsController>>(), _mediator.Object);
        }

        [Test]
        public async Task Then_Will_Filter_Reservations_By_Selected_Course()
        {
            //Arrange
            var selectedCourse = "Test = Level 1";

            //Act
            await _reservationsController.Search(ExpectedProviderId, ExpectedSearchTerm, selectedCourse, null, null);

            //Assert
            _mediator.Verify(m => m.Send(It.Is<FindAccountReservationsQuery>(q => q.SelectedFilters.CourseFilter.Equals(selectedCourse)), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Then_Will_Filter_Reservations_By_Selected_Employer_Name()
        {
            //Arrange
            var selectedEmployerName = "Test Ltd";

            //Act
            await _reservationsController.Search(ExpectedProviderId, ExpectedSearchTerm, null, selectedEmployerName, null);

            //Assert
            _mediator.Verify(m => m.Send(It.Is<FindAccountReservationsQuery>(q => q.SelectedFilters.EmployerNameFilter.Equals(selectedEmployerName)), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Then_Will_Filter_Reservations_By_Selected_Start_Date()
        {
            //Arrange
            var selectedStartDate = DateTime.Now.ToString("g");

            //Act
            await _reservationsController.Search(ExpectedProviderId, ExpectedSearchTerm, null, null, selectedStartDate);

            //Assert
            _mediator.Verify(m => m.Send(It.Is<FindAccountReservationsQuery>(q => q.SelectedFilters.StartDateFilter.Equals(selectedStartDate)), It.IsAny<CancellationToken>()));
        }


        [Test]
        public async Task Then_The_Reservations_Are_Returned()
        {
            //Act
            var actual = await _reservationsController.Search(ExpectedProviderId, ExpectedSearchTerm, null, null, null);

            //Assert
            actual.Should().NotBeNull();

            var result = actual.Should().BeAssignableTo<ObjectResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().NotBeNull();

            var actualReservations = result.Value.Should().BeAssignableTo<FindAccountReservationsResult>().Subject;
            actualReservations.Reservations.Should().BeEquivalentTo(_accountReservationsResult.Reservations);
            actualReservations.NumberOfRecordsFound.Should().Be(_accountReservationsResult.NumberOfRecordsFound);
            actualReservations.Filters.Should().BeEquivalentTo(_accountReservationsResult.Filters);
        }

        [Test]
        public async Task Then_If_A_Validation_Error_Occurs_A_Bad_Request_Is_Returned_With_Errors()
        {
            //Arrange
            var expectedValidationMessage = "The following parameters have failed validation";
            var expectedParam = "ProviderId";
            _mediator.Setup(x => x.Send(It.IsAny<FindAccountReservationsQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(expectedValidationMessage, expectedParam));
            
            //Act
            var actual = await _reservationsController.Search(0, "test", null, null, null);

            //Assert
            var result = actual.Should().BeAssignableTo<ObjectResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            var actualError = result.Value.Should().BeOfType<ArgumentErrorViewModel>().Subject;
            actualError.Message.Should().Be($"{expectedValidationMessage} (Parameter '{expectedParam}')");
            actualError.Params.Should().Be(expectedParam);
        }
    }
}
