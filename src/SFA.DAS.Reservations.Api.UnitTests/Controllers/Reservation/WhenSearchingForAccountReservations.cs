using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;

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
                    AccountLegalEntityFilters = new [] {"Test Ltd", "Acme Bank"}
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
            var actual = await _reservationsController.Search(ExpectedProviderId, ExpectedSearchTerm, selectedCourse, null);

            //Assert
            _mediator.Verify(m => m.Send(It.Is<FindAccountReservationsQuery>(q => q.SelectedFilters.CourseFilter.Equals(selectedCourse)), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task Then_Will_Filter_Reservations_By_Selected_Employer_Name()
        {
            //Arrange
            var selectedEmployerName = "Test Ltd";

            //Act
            var actual = await _reservationsController.Search(ExpectedProviderId, ExpectedSearchTerm, null, selectedEmployerName);

            //Assert
            _mediator.Verify(m => m.Send(It.Is<FindAccountReservationsQuery>(q => q.SelectedFilters.EmployerNameFilter.Equals(selectedEmployerName)), It.IsAny<CancellationToken>()));
        }


        [Test]
        public async Task Then_The_Reservations_Are_Returned()
        {
            //Act
            var actual = await _reservationsController.Search(ExpectedProviderId, ExpectedSearchTerm, null, null);

            //Assert
            Assert.IsNotNull(actual);
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.IsNotNull(result.Value);
            var actualReservations = result.Value as FindAccountReservationsResult;
            Assert.AreEqual(_accountReservationsResult.Reservations, actualReservations.Reservations);
            Assert.AreEqual(_accountReservationsResult.NumberOfRecordsFound, actualReservations.NumberOfRecordsFound);
            Assert.AreEqual(_accountReservationsResult.Filters, actualReservations.Filters);
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
            var actual = await _reservationsController.Search(0, "test", null, null);

            //Assert
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            var actualError = result.Value as ArgumentErrorViewModel;
            Assert.IsNotNull(actualError);
            Assert.AreEqual($"{expectedValidationMessage}{Environment.NewLine}Parameter name: {expectedParam}", actualError.Message);
            Assert.AreEqual(expectedParam, actualError.Params);
        }
    }
}
