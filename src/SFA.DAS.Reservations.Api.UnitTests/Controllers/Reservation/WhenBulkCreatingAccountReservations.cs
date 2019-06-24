using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Reservation
{
    public class WhenBulkCreatingAccountReservations
    {
        private ReservationsController _reservationsController;
        private Mock<IMediator> _mediator;
        private Mock<HttpContext> _httpContext;
        private BulkCreateAccountReservationsResult _bulkCreateAccountReservationsResult;
        private const long ExpectedAccountLegalEntityId = 13124;
        private const int ExpectedReservationCount = 3;

        [SetUp]
        public void Arrange()
        {
            _bulkCreateAccountReservationsResult = new BulkCreateAccountReservationsResult
                { ReservationIds = new List<Guid>{Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()}};
            
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.Is<BulkCreateAccountReservationsCommand>(c =>
                        c.ReservationCount.Equals(ExpectedReservationCount) &&
                        c.AccountLegalEntityId.Equals(ExpectedAccountLegalEntityId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_bulkCreateAccountReservationsResult);
            _httpContext = new Mock<HttpContext>();
            _reservationsController = new ReservationsController(Mock.Of<ILogger<ReservationsController>>(), _mediator.Object)
            {
                ControllerContext = {HttpContext = _httpContext.Object,
                    ActionDescriptor = new ControllerActionDescriptor
                    {
                        ControllerName = "reservations"
                    }}
            };
        }

        [Test]
        public async Task Then_The_New_ReservationIds_Are_Returned_When_Created()
        {
           //Act
            var actual = await _reservationsController.BulkCreate(ExpectedAccountLegalEntityId, ExpectedReservationCount);

            //Assert
            _mediator.Verify(m => m.Send(It.Is<BulkCreateAccountReservationsCommand>(command =>
                    command.ReservationCount.Equals(ExpectedReservationCount) &&
                    command.AccountLegalEntityId.Equals(ExpectedAccountLegalEntityId)),
                It.IsAny<CancellationToken>()), Times.Once);

            Assert.IsNotNull(actual);
            var result = actual as CreatedResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.Created, (HttpStatusCode)result.StatusCode);
            Assert.IsNotNull(result.Value);
            var actualReservations = result.Value as BulkCreateAccountReservationsResult;
            Assert.IsNotNull(actualReservations?.ReservationIds);
            Assert.AreEqual(ExpectedReservationCount, actualReservations.ReservationIds.ToList().Count);
        }

        [Test]
        public async Task Then_If_There_Is_A_Validation_Error_A_Bad_Request_Is_Returned_With_Errors()
        {
            //Arrange
            var expectedValidationMessage = "The following parameters have failed validation";
            var expectedParam = "AccountLegalEntityId";
            _mediator.Setup(x => x.Send(It.IsAny<BulkCreateAccountReservationsCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(expectedValidationMessage, expectedParam));

            //Act
            var actual = await _reservationsController.BulkCreate(ExpectedAccountLegalEntityId, ExpectedReservationCount);

            //Assert
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            var actualError = result.Value as ArgumentErrorViewModel;
            Assert.IsNotNull(actualError);
            Assert.AreEqual($"{expectedValidationMessage}\r\nParameter name: {expectedParam}", actualError.Message);
            Assert.AreEqual(expectedParam, actualError.Params);
        }

        [Test]
        public async Task Then_If_The_AccountLegalEntity_Does_Not_Exist_A_Not_Found_Result_Is_Returned()
        {
            //Arrange
            var expectedValidationMessage = "The following parameters have failed validation";
            _mediator.Setup(x => x.Send(It.IsAny<BulkCreateAccountReservationsCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new EntityNotFoundException<Domain.Entities.AccountLegalEntity>(expectedValidationMessage));

            //Act
            var actual = await _reservationsController.BulkCreate(ExpectedAccountLegalEntityId, ExpectedReservationCount);

            //Assert
            var result = actual as NotFoundResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.NotFound, (HttpStatusCode)result.StatusCode);
        }
    }
}
