using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntities;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.AccountLegalEntities
{
    public class WhenGettingAccountLegalEntitiesForAnAccount
    {
        private AccountLegalEntitiesController _accountLegalEntitiesController;
        private Mock<IMediator> _mediator;
        private GetAccountLegalEntitiesResponse _accountLegalEntitiesResponse;
        private const long ExpectedAccountId = 123234;
        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _accountLegalEntitiesResponse = new GetAccountLegalEntitiesResponse { AccountLegalEntities = new List<Domain.AccountLegalEntities.AccountLegalEntity>() };

            _mediator.Setup(x => x.Send(It.Is<GetAccountLegalEntitiesQuery>(c => c.AccountId.Equals(ExpectedAccountId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_accountLegalEntitiesResponse);

            _accountLegalEntitiesController = new AccountLegalEntitiesController(_mediator.Object, Mock.Of<ILogger<AccountLegalEntitiesController>>());
        }

        [Test]
        public async Task Then_The_AccountLegalEntities_Are_Returned()
        {
            //Act
            var actual = await _accountLegalEntitiesController.GetByAccountId(ExpectedAccountId);

            //Assert
            actual.Should().NotBeNull();

            var result = actual.Should().BeAssignableTo<ObjectResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().NotBeNull();

            var actualAccountLegalEntities = result.Value.Should().BeOfType<List<Domain.AccountLegalEntities.AccountLegalEntity>>().Subject;
            actualAccountLegalEntities.Should().BeEquivalentTo(_accountLegalEntitiesResponse.AccountLegalEntities);

        }

        [Test]
        public async Task Then_If_A_Validation_Error_Occurs_A_Bad_Request_Is_Returned_With_Errors()
        {
            //Arrange
            var expectedValidationMessage = "The following parameters have failed validation";
            var expectedParam = "AccountId";
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountLegalEntitiesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(expectedValidationMessage, expectedParam));

            //Act
            var actual = await _accountLegalEntitiesController.GetByAccountId(0);

            //Assert
            var result = actual.Should().BeAssignableTo<ObjectResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            var actualError = result.Value.Should().BeOfType<ArgumentErrorViewModel>().Subject;
            actualError.Message.Should().Be($"{expectedValidationMessage} (Parameter '{expectedParam}')");
            actualError.Params.Should().Be(expectedParam);

        }
    }
}
