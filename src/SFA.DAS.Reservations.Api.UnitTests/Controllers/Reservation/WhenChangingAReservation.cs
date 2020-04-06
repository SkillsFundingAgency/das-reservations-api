using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.ChangeOfParty;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Reservation
{
    public class WhenChangingAReservation
    {
        [Test, MoqAutoData]
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

        [Test, MoqAutoData]
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

        [Test, MoqAutoData]
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

        [Test, MoqAutoData]
        public async Task And_InvalidOperationException_Then_Returns_Http_Bad_Request(
            Guid reservationId,
            ChangeOfPartyRequest request,
            InvalidOperationException invalidOperationException,
            [Frozen] Mock<IMediator> mockMediator,
            ReservationsController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<ChangeOfPartyCommand>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(invalidOperationException);

            var result = await controller.Change(reservationId, request) as BadRequestObjectResult;

            var model = result.Value as ArgumentErrorViewModel;
            model.Message.Should().Be(invalidOperationException.Message);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_Http_OK_With_New_ReservationId(
            Guid reservationId,
            ChangeOfPartyRequest request,
            ChangeOfPartyResult mediatorResult,
            [Frozen] Mock<IMediator> mockMediator,
            ReservationsController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<ChangeOfPartyCommand>(command => 
                        command.ReservationId == reservationId &&
                        command.AccountLegalEntityId == request.AccountLegalEntityId &&
                        command.ProviderId == request.ProviderId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mediatorResult);

            var result = await controller.Change(reservationId, request) as OkObjectResult;

            var model = result.Value as ChangeOfPartyResponse;
            model.ReservationId.Should().Be(mediatorResult.ReservationId);
        }
    }
}