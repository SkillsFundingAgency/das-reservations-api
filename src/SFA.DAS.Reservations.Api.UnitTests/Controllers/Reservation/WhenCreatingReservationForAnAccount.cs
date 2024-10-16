using System;
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
        private Guid _expectedUserId;

        [SetUp]
        public void Arrange()
        {
            _expectedUserId = Guid.NewGuid();

            _accountReservationsResult = new CreateAccountReservationResult
            {
                Reservation = new Domain.Reservations.Reservation(null,_expectedReservationId,ExpectedAccountId,false,DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow,ReservationStatus.Pending, 
                    new Course(),0,0,"",0,null),
                AgreementSigned = true
            };
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
                TransferSenderAccountId = ExpectedTransferSenderAccountId,
                UserId = _expectedUserId
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
                    command.UserId.Equals(_expectedUserId) &&
                    command.TransferSenderAccountId.Equals(ExpectedTransferSenderAccountId)
                    ), 
                It.IsAny<CancellationToken>()), Times.Once);

            actual.Should().NotBeNull();

            var result = actual.Should().BeOfType<CreatedResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
            result.Value.Should().NotBeNull();

            var actualReservations = result.Value.Should().BeOfType<Domain.Reservations.Reservation>().Subject;
            actualReservations.Should().BeEquivalentTo(_accountReservationsResult.Reservation);
            result.Location.Should().Be($"api/reservations/{_expectedReservationId}");
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
            var result = actual.Should().BeAssignableTo<ObjectResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            var actualError = result.Value.Should().BeAssignableTo<ArgumentErrorViewModel>().Subject;
            actualError.Message.Should().Be($"{expectedValidationMessage} (Parameter '{expectedParam}')");
            actualError.Params.Should().Be(expectedParam);
        }

        [Test]
        public async Task Then_If_There_Are_Global_Rules_Failures_A_Unprocessable_Entity_Response_Is_Returned()
        {
            //Arrange
            _mediator.Setup(x => x.Send(It.IsAny<CreateAccountReservationCommand>(),It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateAccountReservationResult
                {
                    Reservation = null,
                    Rule = new Domain.Rules.GlobalRule(new Domain.Entities.GlobalRule()),
                    AgreementSigned = true
                });


            //Act
            var actual = await _reservationsController.Create(new Api.Models.Reservation());

            //Assert
            var result = actual.Should().BeOfType<UnprocessableEntityObjectResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);

            var errors = result.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().NotBeNull();
            errors.FirstOrDefault().Should().NotBeNull();
        }

        [Test]
        public async Task Then_If_There_Is_A_NonLevy_With_Unsigned_Agreement_An_Unprocessable_Entity_Response_Is_Returned()
        {
            //Arrange
            _mediator.Setup(x => x.Send(It.IsAny<CreateAccountReservationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateAccountReservationResult
                {
                    Reservation = null,
                    Rule = null,
                    AgreementSigned = false
                });


            //Act
            var actual = await _reservationsController.Create(new Api.Models.Reservation());

            //Assert
            var result = actual.Should().BeOfType<UnprocessableEntityObjectResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);

            var errors = result.Value.Should().BeOfType<SerializableError>().Subject;
            errors.Should().NotBeNull();
            errors.FirstOrDefault().Should().NotBeNull();
        }
    }
}
