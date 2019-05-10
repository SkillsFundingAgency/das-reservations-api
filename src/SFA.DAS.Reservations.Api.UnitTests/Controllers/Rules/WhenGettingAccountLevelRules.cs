using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Rules
{
    public class WhenGettingAccountLevelRules
    {
        private Mock<IMediator> _mediator;
        private RulesController _rulesController;
        private GetAccountRulesResult _rulesResult;
        private const long AccountId = 543543;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _rulesResult = new GetAccountRulesResult { GlobalRules = new List<GlobalRule>() };
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountRulesQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_rulesResult);

            _rulesController = new RulesController(_mediator.Object);
        }

        [Test]
        public async Task Then_The_Account_Rules_Are_Returned()
        {
            //Act
            var actual = await _rulesController.Account(AccountId);

            //Assert
            Assert.IsNotNull(actual);
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.IsNotNull(result.Value);
            var actualRules = result.Value as GetAccountRulesResult;
            Assert.AreEqual(_rulesResult.GlobalRules, actualRules.GlobalRules);
        }

        [Test]
        public async Task Then_If_There_Is_A_Validation_Error_A_Bad_Request_Is_Returned_With_Errors()
        {
            //Arrange
            var expectedValidationMessage = "The following parameters have failed validation";
            var expectedParam = "AccountId";
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountRulesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException(expectedValidationMessage, expectedParam));

            //Act
            var actual = await _rulesController.Account(AccountId);

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
