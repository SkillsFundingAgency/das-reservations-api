using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries;

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
            Assert.IsNotNull(actual);
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.IsNotNull(result.Value);
            var actualAccountLegalEntities = result.Value as List<Domain.AccountLegalEntities.AccountLegalEntity>;
            Assert.AreEqual(_accountLegalEntitiesResponse.AccountLegalEntities, actualAccountLegalEntities);
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
