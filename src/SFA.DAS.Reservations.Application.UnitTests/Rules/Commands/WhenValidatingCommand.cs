using System;
using System.Threading.Tasks;
using FluentAssertions;
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
                TypeOfRule = RuleType.GlobalRule
            };

            //Act
            var result = await _validator.ValidateAsync(command);

            //Assert
            result.Should().NotBeNull();
            result.IsValid().Should().BeTrue();
        }

        [Test]
        public async Task ThenWillReturnInValidIfCommandIsInValid()
        {
            //Arrange
            var command = new CreateUserRuleAcknowledgementCommand
            {
                Id = "ABC",
                RuleId = -1,
                TypeOfRule = RuleType.None
            };

            //Act
            var result = await _validator.ValidateAsync(command);

            //Assert
            result.Should().NotBeNull();
            result.IsValid().Should().BeFalse();
            result.ValidationDictionary.Count.Should().Be(3);
            result.ValidationDictionary.Should().ContainKey(nameof(CreateUserRuleAcknowledgementCommand.Id));
            result.ValidationDictionary.Should().ContainKey(nameof(CreateUserRuleAcknowledgementCommand.RuleId));
            result.ValidationDictionary.Should().ContainKey(nameof(CreateUserRuleAcknowledgementCommand.TypeOfRule));

        }

        [Test]
        public async Task ThenWillReturnInValidIfCommandValuesHaveNotBeenSupplied()
        {
            //Arrange
            var command = new CreateUserRuleAcknowledgementCommand();

            //Act
            var result = await _validator.ValidateAsync(command);

            //Assert
            result.Should().NotBeNull();
            result.IsValid().Should().BeFalse();
            result.ValidationDictionary.Should().HaveCount(3);
            result.ValidationDictionary.Should().ContainKeys(
                nameof(CreateUserRuleAcknowledgementCommand.Id),
                nameof(CreateUserRuleAcknowledgementCommand.RuleId),
                nameof(CreateUserRuleAcknowledgementCommand.TypeOfRule)
            );
        }
    }
}
