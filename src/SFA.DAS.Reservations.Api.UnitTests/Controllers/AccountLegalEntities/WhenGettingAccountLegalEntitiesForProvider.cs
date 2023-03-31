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
using SFA.DAS.Reservations.Application.ProviderPermissions.Queries;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.AccountLegalEntities
{
    public class WhenGettingAccountLegalEntitiesForProvider
    {
        public class WhenGettingAccountLegalEntitiesForAnAccount
    {
        private AccountLegalEntitiesController _accountLegalEntitiesController;
        private Mock<IMediator> _mediator;
        private GetAccountLegalEntitiesForProviderResponse _accountLegalEntitiesResponse;
        private const uint ExpectedProviderId = 123234;
        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _accountLegalEntitiesResponse = new GetAccountLegalEntitiesForProviderResponse { ProviderPermissions = new List<Domain.ProviderPermissions.ProviderPermission>() };

            _mediator.Setup(x => x.Send(It.Is<GetAccountLegalEntitiesForProviderQuery>(c => c.ProviderId.Equals(ExpectedProviderId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_accountLegalEntitiesResponse);

            _accountLegalEntitiesController = new AccountLegalEntitiesController(_mediator.Object, Mock.Of<ILogger<AccountLegalEntitiesController>>());
        }

        [Test]
        public async Task Then_The_AccountLegalEntities_Are_Returned()
        {
            //Act
            var actual = await _accountLegalEntitiesController.GetByProviderId(ExpectedProviderId);

            //Assert
            Assert.IsNotNull(actual);
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.IsNotNull(result.Value);
            var actualAccountLegalEntities = result.Value as List<Domain.ProviderPermissions.ProviderPermission>;
            Assert.AreEqual(_accountLegalEntitiesResponse.ProviderPermissions, actualAccountLegalEntities);
        }

        [Test]
        public async Task Then_If_A_Validation_Error_Occurs_A_Bad_Request_Is_Returned_With_Errors()
        {
            //Arrange
            var expectedValidationMessage = "The following parameters have failed validation";
            var expectedParam = "ProviderId";
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountLegalEntitiesForProviderQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(expectedValidationMessage, expectedParam));

            //Act
            var actual = await _accountLegalEntitiesController.GetByProviderId(0);

            //Assert
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            var actualError = result.Value as ArgumentErrorViewModel;
            Assert.IsNotNull(actualError);
            Assert.AreEqual($"{expectedValidationMessage} (Parameter '{expectedParam}')", actualError.Message);
            Assert.AreEqual(expectedParam, actualError.Params);
        }
    }
    }
}