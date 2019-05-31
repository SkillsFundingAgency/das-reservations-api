using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Commands.CreateUserRuleAcknowledgement;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Commands
{
    public class WhenValidatingCommand
    {
        private CreateUserRuleAcknowledgementCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new CreateUserRuleAcknowledgementCommandValidator();
        }

        [Test]
        public async Task ThenWillReturnValidIfCommandIsValid()
        {
            //Arrange
            var command = new CreateUserRuleAcknowledgementCommand
            {
                Id = Guid.NewGuid().ToString(),
                RuleId = 1234,
                RuleType = RuleType.GlobalRule
            };

            //Act
            var result = await _validator.ValidateAsync(command);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public async Task ThenWillReturnInValidIfCommandIsInValid()
        {
            //Arrange
            var command = new CreateUserRuleAcknowledgementCommand
            {
                Id = "ABC",
                RuleId = -1,
                RuleType = RuleType.None
            };

            //Act
            var result = await _validator.ValidateAsync(command);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid());
            Assert.AreEqual(3, result.ValidationDictionary.Count);
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(CreateUserRuleAcknowledgementCommand.Id)));
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(CreateUserRuleAcknowledgementCommand.RuleId)));
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(CreateUserRuleAcknowledgementCommand.RuleType)));
        }

        [Test]
        public async Task ThenWillReturnInValidIfCommandValuesHaveNotBeenSupplied()
        {
            //Arrange
            var command = new CreateUserRuleAcknowledgementCommand();

            //Act
            var result = await _validator.ValidateAsync(command);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid());
            Assert.AreEqual(3, result.ValidationDictionary.Count);
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(CreateUserRuleAcknowledgementCommand.Id)));
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(CreateUserRuleAcknowledgementCommand.RuleId)));
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(CreateUserRuleAcknowledgementCommand.RuleType)));
        }
    }
}
