using System;
using System.Net;
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
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.AccountLegalEntities
{
    [TestFixture]
    public class WhenGettingAccountReservationStatus
    {
        [Test, MoqAutoData]
        public async Task And_ArgumentException_Then_Returns_BadRequest(
            long accountId,
            ArgumentException argumentException,
            [Frozen] Mock<IMediator> mockMediator,
            [NoAutoProperties] AccountLegalEntitiesController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetAccountReservationStatusQuery>(query => query.AccountId == accountId),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(argumentException);

            var result = await controller.GetAccountReservationStatus(accountId) as BadRequestObjectResult;

            result.StatusCode.Should().Be((int) HttpStatusCode.BadRequest);
            var model = result.Value.Should().BeOfType<ArgumentErrorViewModel>().Subject;
            model.Message.Should().Be(argumentException.Message);
            model.Params.Should().Be(argumentException.ParamName);
        }

        [Test, MoqAutoData]
        public async Task And_EntityNotFoundException_Then_Returns_NotFound(
            long accountId,
            EntityNotFoundException<AccountLegalEntity> notFoundException,
            [Frozen] Mock<IMediator> mockMediator,
             [NoAutoProperties] AccountLegalEntitiesController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetAccountReservationStatusQuery>(query => query.AccountId == accountId),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(notFoundException);

            var result = await controller.GetAccountReservationStatus(accountId) as NotFoundResult;

            result.StatusCode.Should().Be((int) HttpStatusCode.NotFound);
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_AccountReservationStatus(
            long accountId,
            GetAccountReservationStatusResponse response,
            [Frozen] Mock<IMediator> mockMediator,
             [NoAutoProperties] AccountLegalEntitiesController controller)
        {
            mockMediator
                .Setup(mediator => mediator.Send(
                    It.Is<GetAccountReservationStatusQuery>(query => query.AccountId == accountId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            
            var result = await controller.GetAccountReservationStatus(accountId) as OkObjectResult;

            result.StatusCode.Should().Be((int) HttpStatusCode.OK);
            var model = result.Value as AccountReservationStatus;
            model.Should().BeEquivalentTo(new AccountReservationStatus(response));
        }
    }
}