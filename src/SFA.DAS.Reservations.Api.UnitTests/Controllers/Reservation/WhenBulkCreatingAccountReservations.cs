using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
using SFA.DAS.Reservations.Domain.Exceptions;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Reservation
{
    public class WhenBulkCreatingAccountReservations
    {
        private ReservationsController _reservationsController;
        private Mock<IMediator> _mediator;
        private Mock<HttpContext> _httpContext;
        private BulkCreateAccountReservationsResult _bulkCreateAccountReservationsResult;
        private BulkReservation _bulkReservation;
        private const long ExpectedAccountLegalEntityId = 13124;
        private const long ExpectedTransferSenderId = 3245;
        private const int ExpectedReservationCount = 3;

        [SetUp]
        public void Arrange()
        {
            _bulkReservation = new BulkReservation
            {
                Count = ExpectedReservationCount,
                TransferSenderId = ExpectedTransferSenderId
            };
            _bulkCreateAccountReservationsResult = new BulkCreateAccountReservationsResult
                { ReservationIds = new List<Guid>{Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()}};
            
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.Is<BulkCreateAccountReservationsCommand>(c =>
                        c.ReservationCount.Equals(ExpectedReservationCount) &&
                        c.AccountLegalEntityId.Equals(ExpectedAccountLegalEntityId) &&
                        c.TransferSenderAccountId.Equals(ExpectedTransferSenderId)),
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
            var actual = await _reservationsController.BulkCreate(ExpectedAccountLegalEntityId, _bulkReservation);

            //Assert
            _mediator.Verify(m => m.Send(It.Is<BulkCreateAccountReservationsCommand>(command =>
                    command.ReservationCount.Equals(ExpectedReservationCount) &&
                    command.AccountLegalEntityId.Equals(ExpectedAccountLegalEntityId) &&
                    command.TransferSenderAccountId.Equals(ExpectedTransferSenderId)),
                It.IsAny<CancellationToken>()), Times.Once);

            actual.Should().NotBeNull();

            var result = actual.Should().BeOfType<CreatedResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
            result.Value.Should().NotBeNull();

            var actualReservations = result.Value.Should().BeOfType<BulkCreateAccountReservationsResult>().Subject;
            actualReservations.ReservationIds.Should().NotBeNull();
            actualReservations.ReservationIds.Should().HaveCount(ExpectedReservationCount);

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
            var actual = await _reservationsController.BulkCreate(ExpectedAccountLegalEntityId, _bulkReservation);

            //Assert
            var result = actual.Should().BeAssignableTo<ObjectResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            var actualError = result.Value.Should().BeAssignableTo<ArgumentErrorViewModel>().Subject;
            actualError.Message.Should().Be($"{expectedValidationMessage} (Parameter '{expectedParam}')");
            actualError.Params.Should().Be(expectedParam);

        }

        [Test]
        public async Task Then_If_The_AccountLegalEntity_Does_Not_Exist_A_Not_Found_Result_Is_Returned()
        {
            //Arrange
            var expectedValidationMessage = "The following parameters have failed validation";
            _mediator.Setup(x => x.Send(It.IsAny<BulkCreateAccountReservationsCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new EntityNotFoundException<Domain.Entities.AccountLegalEntity>(expectedValidationMessage));

            //Act
            var actual = await _reservationsController.BulkCreate(ExpectedAccountLegalEntityId, _bulkReservation);

            //Assert
            var result = actual as NotFoundResult;
            result.Should().NotBeNull();
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
