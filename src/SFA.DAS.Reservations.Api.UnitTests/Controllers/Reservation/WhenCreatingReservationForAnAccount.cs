using System;
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
using SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Reservation
{
    public class WhenCreatingReservationForAnAccount
    {
        private ReservationsController _reservationsController;
        private Mock<IMediator> _mediator;
        private CreateAccountReservationResult _accountReservationsResult;
        private const long ExpectedAccountId = 123234;
        private const string ExpectedAccountLegalEntityName= "TestName";
        private readonly long _expectedAccountLegalEntityId = 18723918;
        private readonly uint? _expectedProviderId = 18723918;
        private readonly long ExpectedTransferSenderAccountId = 184513;
        private readonly Guid _expectedReservationId = Guid.NewGuid();
        private readonly DateTime _expectedStartDate = new DateTime(2018,5,24);
        private readonly string _expectedCourseId = "asdfopi";
        private readonly bool ExpectedIsLevyAccount = true;
        private Mock<HttpContext> _httpContext;

        [SetUp]
        public void Arrange()
        {
            _accountReservationsResult = new CreateAccountReservationResult
                {Reservation = new Domain.Reservations.Reservation(null,_expectedReservationId,ExpectedAccountId,false,DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow,ReservationStatus.Pending, new Course(),0,0,"",0)};
            ;
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.Is<CreateAccountReservationCommand>(c => 
                        c.Id.Equals(_expectedReservationId) &&
                        c.AccountId.Equals(ExpectedAccountId) && 
                        c.StartDate.Equals(_expectedStartDate)),
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
        public async Task Then_The_Reservation_Is_Created_From_The_Request_And_Returned()
        {
            //Arrange
            var reservation = new Api.Models.Reservation
            {
                Id = _expectedReservationId,
                AccountId = ExpectedAccountId, 
                StartDate = _expectedStartDate,
                CourseId = _expectedCourseId,
                ProviderId = _expectedProviderId,
                AccountLegalEntityId = _expectedAccountLegalEntityId,
                AccountLegalEntityName = ExpectedAccountLegalEntityName,
                IsLevyAccount =  ExpectedIsLevyAccount,
                TransferSenderAccountId = ExpectedTransferSenderAccountId
            };


            //Act
            var actual = await _reservationsController.Create(reservation);

            //Assert
            _mediator.Verify(m => m.Send(It.Is<CreateAccountReservationCommand>(command => 
                    command.CourseId.Equals(_expectedCourseId) &&
                    command.ProviderId.Equals(_expectedProviderId) &&
                    command.AccountLegalEntityId.Equals(_expectedAccountLegalEntityId) &&
                    command.AccountId.Equals(ExpectedAccountId) &&
                    command.StartDate.Equals(_expectedStartDate) &&
                    command.Id.Equals(_expectedReservationId) &&
                    command.IsLevyAccount.Equals(ExpectedIsLevyAccount) &&
                    command.AccountLegalEntityName.Equals(ExpectedAccountLegalEntityName) &&
                    command.TransferSenderAccountId.Equals(ExpectedTransferSenderAccountId)
                    ), 
                It.IsAny<CancellationToken>()), Times.Once);

            Assert.IsNotNull(actual);
            var result = actual as CreatedResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.Created, (HttpStatusCode)result.StatusCode);
            Assert.IsNotNull(result.Value);
            var actualReservations = result.Value as Domain.Reservations.Reservation;
            Assert.AreEqual(_accountReservationsResult.Reservation, actualReservations);
            Assert.AreEqual($"api/reservations/{_expectedReservationId}", result.Location);
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
            var actual = await _reservationsController.Create(new Api.Models.Reservation());

            //Assert
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            var actualError = result.Value as ArgumentErrorViewModel;
            Assert.IsNotNull(actualError);
            Assert.AreEqual($"{expectedValidationMessage}{Environment.NewLine}Parameter name: {expectedParam}", actualError.Message);
            Assert.AreEqual(expectedParam, actualError.Params);
        }

        [Test]
        public async Task Then_If_There_Are_Global_Rules_Failures_A_Unprocessable_Entity_Response_Is_Returned()
        {
            //Arrange
            _mediator.Setup(x => x.Send(It.IsAny<CreateAccountReservationCommand>(),It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateAccountReservationResult
                {
                    Reservation = null,
                    Rule = new Domain.Rules.GlobalRule(new Domain.Entities.GlobalRule())
                });


            //Act
            var actual = await _reservationsController.Create(new Api.Models.Reservation());

            //Assert
            var result = actual as UnprocessableEntityObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, (HttpStatusCode)result.StatusCode);
            var errors = result.Value as SerializableError;
            Assert.IsNotNull(errors?.FirstOrDefault());
        }

        [Test]
        public async Task Then_If_There_Is_A_NonLevy_With_No_Eoi_Agreement_And_Unprocessable_Entity_Response_Is_Returned()
        {
            //Arrange
            _mediator.Setup(x => x.Send(It.IsAny<CreateAccountReservationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateAccountReservationResult
                {
                    Reservation = null,
                    Rule = null,
                    NonLevyNonEoiAgreementSigned = true
                });


            //Act
            var actual = await _reservationsController.Create(new Api.Models.Reservation());

            //Assert
            var result = actual as UnprocessableEntityObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.UnprocessableEntity, (HttpStatusCode)result.StatusCode);
            var errors = result.Value as SerializableError;
            Assert.IsNotNull(errors?.FirstOrDefault());
        }
    }
}
