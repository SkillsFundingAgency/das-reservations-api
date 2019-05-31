using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.Rules.Commands.CreateUserRuleAcknowledgement
{
    public class CreateUserRuleAcknowledgementCommandValidator : IValidator<CreateUserRuleAcknowledgementCommand>
    {
        public Task<ValidationResult> ValidateAsync(CreateUserRuleAcknowledgementCommand command)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(command.Id))
            {
                validationResult.AddError(nameof(CreateUserRuleAcknowledgementCommand.Id), $"{nameof(CreateUserRuleAcknowledgementCommand.Id)} has not be set");
            }
            else if (!Guid.TryParse(command.Id, out _) && !long.TryParse(command.Id, out _))
            {
                validationResult.AddError(nameof(CreateUserRuleAcknowledgementCommand.Id), $"{nameof(CreateUserRuleAcknowledgementCommand.Id)} valid is not valid");
            }

            if (command.RuleId == 0)
            {
                validationResult.AddError(nameof(CreateUserRuleAcknowledgementCommand.RuleId), $"{nameof(CreateUserRuleAcknowledgementCommand.RuleId)} has not be set");
            }
            else if (command.RuleId < 0)
            {
                validationResult.AddError(nameof(CreateUserRuleAcknowledgementCommand.RuleId), $"{nameof(CreateUserRuleAcknowledgementCommand.RuleId)} valid is not valid");
            }

            if (command.RuleType == RuleType.None)
            {
                validationResult.AddError(nameof(CreateUserRuleAcknowledgementCommand.RuleType), $"{nameof(CreateUserRuleAcknowledgementCommand.RuleType)} has not be set");
            }

            return Task.FromResult(validationResult);
        }
    }
}
