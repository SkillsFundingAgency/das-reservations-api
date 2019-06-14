using System;
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
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Reservation
{
    public class WhenGettingAReservation
    {
        private ReservationsController _reservationsController;
        private Mock<IMediator> _mediator;
        private GetReservationResponse _accountReservationsResult;
        private const long ExpectedAccountId = 123234;
        private readonly Guid _expectedReservationId = Guid.NewGuid();
        private Mock<HttpContext> _httpContext;

        [SetUp]
        public void Arrange()
        {
            _accountReservationsResult = new GetReservationResponse
            {
                Reservation = new Domain.Reservations.Reservation(null, _expectedReservationId, ExpectedAccountId, false, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, ReservationStatus.Pending, null,0,0,"")
            };
            
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.Is<GetReservationQuery>(c => c.Id.Equals(_expectedReservationId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_accountReservationsResult);
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
        public async Task Then_The_Reservations_Are_Returned()
        {
            //Act
            var actual = await _reservationsController.Get(_expectedReservationId);

            //Assert
            Assert.IsNotNull(actual);
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.IsNotNull(result.Value);
            var actualReservations = result.Value as Domain.Reservations.Reservation;
            Assert.AreEqual(_accountReservationsResult.Reservation, actualReservations);
        }

        [Test]
        public async Task Then_If_A_Validation_Error_Occurs_A_Bad_Request_Is_Returned_With_Errors()
        {
            //Arrange
            var expectedValidationMessage = "The following parameters have failed validation";
            var expectedParam = "Id";
            _mediator.Setup(x => x.Send(It.IsAny<GetReservationQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(expectedValidationMessage, expectedParam));

            //Act
            var actual = await _reservationsController.Get(Guid.Empty);

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
        public async Task Then_A_Not_Found_Result_Is_Returned_If_Null_Is_Returned_From_The_Handler()
        {
            //Act
            var actual = await _reservationsController.Get(Guid.NewGuid());

            //Assert
            var result = actual as NotFoundResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.NotFound, (HttpStatusCode)result.StatusCode);
        }
    }
}
