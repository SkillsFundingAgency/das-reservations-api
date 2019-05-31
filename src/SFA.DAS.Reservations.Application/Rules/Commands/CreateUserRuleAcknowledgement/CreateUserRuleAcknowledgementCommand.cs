using MediatR;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Commands.CreateUserRuleAcknowledgement
{
    public class CreateUserRuleAcknowledgementCommand : IRequest<Unit>, IUserRuleAcknowledgementRequest
    {
        public string Id { get; set; }
        public long RuleId { get; set; }
        public RuleType TypeOfRule { get; set; }
    }
}
