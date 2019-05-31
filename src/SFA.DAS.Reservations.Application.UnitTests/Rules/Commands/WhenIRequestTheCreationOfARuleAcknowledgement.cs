using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Commands.CreateUserRuleAcknowledgement;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;
using ValidationResult = SFA.DAS.Reservations.Domain.Validation.ValidationResult;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Commands
{
    public class WhenIRequestTheCreationOfARuleAcknowledgement
    {
        private Mock<IUserRuleAcknowledgementService> _service;
        private CreateUserRuleAcknowledgementCommandHandler _handler;
        private CreateUserRuleAcknowledgementCommand _command;
        private Mock<IValidator<CreateUserRuleAcknowledgementCommand>> _validator;

        [SetUp]
        public void Arrange()
        {
            _command = new CreateUserRuleAcknowledgementCommand
            {
                Id = "123",
                RuleId = 1234,
                RuleType = RuleType.GlobalRule
            };

            _validator = new Mock<IValidator<CreateUserRuleAcknowledgementCommand>>();
            _service = new Mock<IUserRuleAcknowledgementService>();
            _handler = new CreateUserRuleAcknowledgementCommandHandler(_service.Object, _validator.Object);

            _validator.Setup(v => v.ValidateAsync(It.IsAny<CreateUserRuleAcknowledgementCommand>()))
                      .ReturnsAsync(() => new ValidationResult());
        }

        [Test]
        public async Task ThenWillCreateUserAcknowledgement()
        {
            //Act
            await _handler.Handle(_command, CancellationToken.None);

            //Assert
            _service.Verify(s => s.CreateUserRuleAcknowledgement(It.Is<IUserRuleAcknowledgementRequest>(
                request => request.Id.Equals(_command.Id) &&
                           request.RuleId.Equals(_command.RuleId) &&
                           request.RuleType.Equals(_command.RuleType))), Times.Once);
        }

        [Test] 
        public async Task ThenWillValidateCommand()
        {
            //Act
            await _handler.Handle(_command, CancellationToken.None);

            //Assert
            _validator.Verify(v => v.ValidateAsync(_command), Times.Once);
        }

        [Test] 
        public void ThenWillThrowExceptionIfValidationFails()
        {
            //Arrange
            _validator.Setup(v => v.ValidateAsync(It.IsAny<CreateUserRuleAcknowledgementCommand>()))
                .ReturnsAsync(() => new ValidationResult{ValidationDictionary = new Dictionary<string, string>{{"Error", "Description"}}});

            //Act
            Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(_command, CancellationToken.None));
        }
    }
}
