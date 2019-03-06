using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountReservations.Commands;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Reservation
{
    public class WhenCreatingReservationForAnAccount
    {
        private ReservationsController _reservationsController;
        private Mock<IMediator> _mediator;
        private CreateAccountReservationResult _accountReservationsResult;
        private const long ExpectedAccountId = 123234;
        private readonly Guid _expectedReservationId = Guid.NewGuid();
        private readonly DateTime _expectedStartDate = new DateTime(2018,5,24);
        private Mock<HttpContext> _httpContext;

        [SetUp]
        public void Arrange()
        {
            _accountReservationsResult = new CreateAccountReservationResult
                {Reservation = new Domain.Reservations.Reservation(null,_expectedReservationId,ExpectedAccountId,false,DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow,ReservationStatus.Pending, new Course())};
            ;
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.Is<CreateAccountReservationCommand>(c => c.AccountId.Equals(ExpectedAccountId) && c.StartDate.Equals(_expectedStartDate)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_accountReservationsResult);
            _httpContext = new Mock<HttpContext>();
            _reservationsController = new ReservationsController(_mediator.Object)
            {
                ControllerContext = {HttpContext = _httpContext.Object,
                    ActionDescriptor = new ControllerActionDescriptor
                    {
                        ControllerName = "reservations"
                    }}
            };
        }

        [Test]
        public async Task Then_The_Reservation_Is_Created_And_Returned()
        {
            //Act
            var actual = await _reservationsController.Create(new Models.Reservation{AccountId = ExpectedAccountId, StartDate = _expectedStartDate});

            //Assert
            Assert.IsNotNull(actual);
            var result = actual as CreatedResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.Created, (HttpStatusCode)result.StatusCode);
            Assert.IsNotNull(result.Value);
            var actualReservations = result.Value as Domain.Reservations.Reservation;
            Assert.AreEqual(_accountReservationsResult.Reservation, actualReservations);
            Assert.AreEqual($"api/accounts/123234/reservations/{_expectedReservationId}", result.Location);
        }

        [Test]
        public async Task Then_If_There_Is_A_Validation_Error_A_Bad_Request_Is_Returned_With_Errors()
        {
            //Arrange
            var expectedValidationMessage = "The following parameters have failed validation";
            var expectedParam = "AccountId";
            _mediator.Setup(x => x.Send(It.IsAny<CreateAccountReservationCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(expectedValidationMessage, expectedParam));

            //Act
            var actual = await _reservationsController.Create(new Models.Reservation());

            //Assert
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            var actualError = result.Value as ArgumentErrorViewModel;
            Assert.IsNotNull(actualError);
            Assert.AreEqual($"{expectedValidationMessage}\r\nParameter name: {expectedParam}", actualError.Message);
            Assert.AreEqual(expectedParam, actualError.Params);
        }
    }
}
