using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Api.UnitTests.Controllers.Customisations;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.ChangeOfParty;
using SFA.DAS.Reservations.Domain.Exceptions;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Reservation
{
    public class WhenChangingAReservation
    {
        [Test, DomainAutoData]
        public async Task And_Fails_Validation_Then_Returns_Http_Bad_Request(
            Guid reservationId,
            ChangeOfPartyRequest request,
            ArgumentException argumentException,
            [Frozen] Mock<IMediator> mockMediator,
            ReservationsController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<ChangeOfPartyCommand>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(argumentException);

            var result = await controller.Change(reservationId, request) as BadRequestObjectResult;

            var model = result.Value as ArgumentErrorViewModel;
            model.Message.Should().Be(argumentException.Message);
            model.Params.Should().Be(argumentException.ParamName);
        }

        [Test, DomainAutoData]
        public async Task And_Reservation_Not_Found_Then_Returns_Http_Bad_Request(
            Guid reservationId,
            ChangeOfPartyRequest request,
            EntityNotFoundException<Domain.Entities.Reservation> notFoundException,
            [Frozen] Mock<IMediator> mockMediator,
            ReservationsController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<ChangeOfPartyCommand>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(notFoundException);

            var result = await controller.Change(reservationId, request) as BadRequestObjectResult;

            var model = result.Value as ArgumentErrorViewModel;
            model.Message.Should().Be(notFoundException.Message);
        }

        [Test, DomainAutoData]
        public async Task And_AccountLegalEntity_Not_Found_Then_Returns_Http_Bad_Request(
            Guid reservationId,
            ChangeOfPartyRequest request,
            EntityNotFoundException<Domain.Entities.AccountLegalEntity> notFoundException,
            [Frozen] Mock<IMediator> mockMediator,
            ReservationsController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<ChangeOfPartyCommand>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(notFoundException);

            var result = await controller.Change(reservationId, request) as BadRequestObjectResult;

            var model = result.Value as ArgumentErrorViewModel;
            model.Message.Should().Be(notFoundException.Message);
        }

        [Test, DomainAutoData]
        public async Task Then_Returns_Http_Create_With_New_ReservationId(
            Guid reservationId,
            ChangeOfPartyRequest request,
            ChangeOfPartyResult mediatorResult,
            ControllerActionDescriptor controllerActionDescriptor,
            [Frozen] Mock<IMediator> mockMediator,
            ReservationsController controller)
        {
            controller.ControllerContext.ActionDescriptor = controllerActionDescriptor;
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<ChangeOfPartyCommand>(command => 
                        command.ReservationId == reservationId &&
                        command.AccountLegalEntityId == request.AccountLegalEntityId &&
                        command.ProviderId == request.ProviderId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mediatorResult);

            var result = await controller.Change(reservationId, request) as CreatedResult;

            result.Location.Should().Be($"api/{controllerActionDescriptor.ControllerName}/{mediatorResult.ReservationId}");
            var model = result.Value as ChangeOfPartyResponse;
            model.ReservationId.Should().Be(mediatorResult.ReservationId);
        }
    }
}