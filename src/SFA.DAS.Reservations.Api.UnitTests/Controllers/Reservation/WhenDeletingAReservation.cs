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
using SFA.DAS.Reservations.Application.AccountReservations.Commands.DeleteReservation;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Reservation
{
    [TestFixture]
    public class WhenDeletingAReservation
    {
        [Test, MoqAutoData]
        public async Task And_ArgumentException_Then_Returns_BadRequest(
            Guid reservationId,
            bool employerDeleted,
            ArgumentException argumentException,
            [Frozen] Mock<IMediator> mockMediator,
            ReservationsController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<DeleteReservationCommand>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(argumentException);

            var result = await controller.Delete(reservationId, employerDeleted) as BadRequestObjectResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
            var error = result.Value as ArgumentErrorViewModel;
            error.Should().NotBeNull();
            error.Message.Should().Be(argumentException.Message);
            error.Params.Should().Be(argumentException.ParamName);
        }

        [Test, MoqAutoData]
        public async Task And_EntityNotFoundException_Then_Returns_Gone(
            Guid reservationId,
            bool employerDeleted,
            EntityNotFoundException<Domain.Entities.Reservation> notFoundException,
            [Frozen] Mock<IMediator> mockMediator,
            ReservationsController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<DeleteReservationCommand>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(notFoundException);

            var result = await controller.Delete(reservationId, employerDeleted) as StatusCodeResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(410);
        }

        [Test, MoqAutoData]
        public async Task And_InvalidOperationException_Then_Returns_BadRequest(
            Guid reservationId,
            bool employerDeleted,
            InvalidOperationException invalidOperationException,
            [Frozen] Mock<IMediator> mockMediator,
            ReservationsController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.IsAny<DeleteReservationCommand>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(invalidOperationException);

            var result = await controller.Delete(reservationId, employerDeleted) as BadRequestResult;

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
        }

        [Test, MoqAutoData]
        public async Task And_No_Error_Then_Returns_Ok(
            Guid reservationId,
            bool employerDeleted,
            [Frozen] Mock<IMediator> mockMediator,
            ReservationsController controller)
        {
            var result = await controller.Delete(reservationId, employerDeleted) as NoContentResult;

            mockMediator.Verify(x =>
                x.Send(
                    It.Is<DeleteReservationCommand>(c =>
                        c.ReservationId.Equals(reservationId)
                        && c.EmployerDeleted.Equals(employerDeleted)),
                    It.IsAny<CancellationToken>()), Times.Once);
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(204);
        }
    }
}