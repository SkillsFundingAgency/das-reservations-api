using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Application.Rules.Commands.CreateUserRuleAcknowledgement;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Rules
{
    public class WhenAcknowledgingRule
    {
        private Mock<IMediator> _mediator;
        private RulesController _rulesController;
        private CreateUserRuleAcknowledgementCommand _command;


        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _rulesController = new RulesController(_mediator.Object);

            _command = new CreateUserRuleAcknowledgementCommand
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
            var result = await _rulesController.AcknowledgeRuleAsRead(_command) as OkResult;

            //Assert
            _mediator.Verify(m => m.Send(It.Is<CreateUserRuleAcknowledgementCommand>(
                c => c.Id.Equals(_command.Id) &&
                     c.RuleId.Equals(_command.RuleId) &&
                     c.TypeOfRule.Equals(_command.TypeOfRule)), It.IsAny<CancellationToken>()), Times.Once);
            
            result.Should().NotBeNull();
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
            var actualException = Assert.ThrowsAsync<Exception>(() => _rulesController.AcknowledgeRuleAsRead(_command));

            //Assert
            actualException.Should().Be(expectedException);
        }
    }
}
