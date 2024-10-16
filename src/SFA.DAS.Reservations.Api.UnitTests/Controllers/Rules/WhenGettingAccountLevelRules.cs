using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
            actual.Should().NotBeNull();

            var result = actual.Should().BeAssignableTo<ObjectResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().NotBeNull();

            var actualRules = result.Value.Should().BeAssignableTo<GetAccountRulesResult>().Subject;
            actualRules.GlobalRules.Should().BeEquivalentTo(_rulesResult.GlobalRules);

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
            var result = actual.Should().BeAssignableTo<ObjectResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            var actualError = result.Value.Should().BeAssignableTo<ArgumentErrorViewModel>().Subject;
            actualError.Message.Should().Be($"{expectedValidationMessage} (Parameter '{expectedParam}')");
            actualError.Params.Should().Be(expectedParam);
        }
    }
}
