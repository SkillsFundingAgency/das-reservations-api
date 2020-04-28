﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntity;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.AccountLegalEntities
{
    public class WhenGettingAccountLegalEntityById
    {
        private const long ExpectedLegalEntityId = 123234;

        private AccountLegalEntitiesController _accountLegalEntitiesController;
        private Mock<IMediator> _mediator;
        private GetAccountLegalEntityResult _accountLegalEntityResult;
        private AccountLegalEntity _expectedLegalEntity;
        
        [SetUp]
        public void Arrange()
        {
            _expectedLegalEntity = new AccountLegalEntity(
                Guid.NewGuid(), 
                1, 
                "Test", 
                2,
                3, 
                5,
                true, 
                true);

            _mediator = new Mock<IMediator>();
            _accountLegalEntityResult = new GetAccountLegalEntityResult { LegalEntity  = _expectedLegalEntity};

            _mediator.Setup(x => x.Send(It.Is<GetAccountLegalEntityQuery>(q => q.Id.Equals(ExpectedLegalEntityId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_accountLegalEntityResult);

            _accountLegalEntitiesController = new AccountLegalEntitiesController(_mediator.Object, Mock.Of<ILogger<AccountLegalEntitiesController>>());
        }

        [Test]
        public async Task Then_The_AccountLegalEntities_Are_Returned()
        {
            //Act
            var actual = await _accountLegalEntitiesController.GetByAccountLegalEntityId(ExpectedLegalEntityId);

            //Assert
            Assert.IsNotNull(actual);
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.IsNotNull(result.Value);
            var actualAccountLegalEntity = result.Value as AccountLegalEntity;
            Assert.AreEqual(_accountLegalEntityResult.LegalEntity, actualAccountLegalEntity);
        }

        [Test]
        public async Task Then_If_A_Validation_Error_Occurs_A_Bad_Request_Is_Returned_With_Errors()
        {
            //Arrange
            var expectedValidationMessage = "The following parameters have failed validation";
            var expectedParam = "Id";
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountLegalEntityQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(expectedValidationMessage, expectedParam));

            //Act
            var actual = await _accountLegalEntitiesController.GetByAccountLegalEntityId(0);

            //Assert
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
            var actualError = result.Value as ArgumentErrorViewModel;
            Assert.IsNotNull(actualError);
            Assert.AreEqual($"{expectedValidationMessage}{Environment.NewLine}Parameter name: {expectedParam}", actualError.Message);
            Assert.AreEqual(expectedParam, actualError.Params);
        }
    }
}
