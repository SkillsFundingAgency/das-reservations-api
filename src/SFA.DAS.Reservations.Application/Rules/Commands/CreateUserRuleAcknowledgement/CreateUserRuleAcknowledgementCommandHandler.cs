using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.Rules.Commands.CreateUserRuleAcknowledgement
{
    public class CreateUserRuleAcknowledgementCommandHandler : IRequestHandler<CreateUserRuleAcknowledgementCommand, Unit>
    {
        private readonly IUserRuleAcknowledgementService _acknowledgementService;
        private readonly IValidator<CreateUserRuleAcknowledgementCommand> _validator;

        public CreateUserRuleAcknowledgementCommandHandler(
            IUserRuleAcknowledgementService acknowledgementService,
            IValidator<CreateUserRuleAcknowledgementCommand> validator)
        {
            _acknowledgementService = acknowledgementService;
            _validator = validator;
        }

        public async Task<Unit> Handle(CreateUserRuleAcknowledgementCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(command);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation", validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            await _acknowledgementService.CreateUserRuleAcknowledgement(command);

            return new Unit();
        }
    }
}
