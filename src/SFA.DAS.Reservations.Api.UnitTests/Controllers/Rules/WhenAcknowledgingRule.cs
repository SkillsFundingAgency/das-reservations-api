using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.Rules.Commands.CreateUserRuleAcknowledgement;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Rules
{
    public class WhenAcknowledgingRule
    {
        private Mock<IMediator> _mediator;
        private RulesController _rulesController;
        private UpcomingRule _rule;


        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _rulesController = new RulesController(_mediator.Object);

            _rule = new UpcomingRule
            {
                Id = Guid.NewGuid().ToString(),
                RuleId = 1234,
                TypeOfRule = RuleType.GlobalRule
            };
        }

        [Test]
        public async Task ThenAcknowledgementIsCreated()
        {
            //Act
            var result = await _rulesController.AcknowledgeRuleAsRead(_rule) as OkResult;

            //Assert
            _mediator.Verify(m => m.Send(It.Is<CreateUserRuleAcknowledgementCommand>(
                c => c.Id.Equals(_rule.Id) &&
                     c.RuleId.Equals(_rule.RuleId) &&
                     c.TypeOfRule.Equals(_rule.TypeOfRule)), It.IsAny<CancellationToken>()), Times.Once);

            Assert.IsNotNull(result);
        }

        [Test]
        public void ThenThrowsExceptionIfOneRaised()
        {
            //Arrange
            var expectedException = new Exception();

            _mediator.Setup(m => m.Send(
                It.IsAny<CreateUserRuleAcknowledgementCommand>(), 
                It.IsAny<CancellationToken>()))
                .Throws(expectedException);

            //Act
            var actualException = Assert.ThrowsAsync<Exception>(() => _rulesController.AcknowledgeRuleAsRead(_rule));

            //Assert
            Assert.AreEqual(expectedException, actualException);
        }
    }
}
