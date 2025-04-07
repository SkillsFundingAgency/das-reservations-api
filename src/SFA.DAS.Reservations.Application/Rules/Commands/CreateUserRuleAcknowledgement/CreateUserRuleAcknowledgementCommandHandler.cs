using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.Rules.Commands.CreateUserRuleAcknowledgement
{
    public class CreateUserRuleAcknowledgementCommandHandler(
        IUserRuleAcknowledgementService acknowledgementService,
        IValidator<CreateUserRuleAcknowledgementCommand> validator)
        : IRequestHandler<CreateUserRuleAcknowledgementCommand, Unit>
    {
        public async Task<Unit> Handle(CreateUserRuleAcknowledgementCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation", validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            await acknowledgementService.CreateUserRuleAcknowledgement(command);

            return new Unit();
        }
    }
}
