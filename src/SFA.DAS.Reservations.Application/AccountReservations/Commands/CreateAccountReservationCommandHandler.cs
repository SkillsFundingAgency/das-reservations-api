using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.UnitOfWork;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands
{
    public class CreateAccountReservationCommandHandler : IRequestHandler<CreateAccountReservationCommand, CreateAccountReservationResult>
    {
        private readonly IAccountReservationService _accountReservationService;
        private readonly IValidator<CreateAccountReservationCommand> _validator;
        private readonly IGlobalRulesService _globalRulesService;
        private readonly IUnitOfWorkContext _context;

        public CreateAccountReservationCommandHandler(IAccountReservationService accountReservationService,
            IValidator<CreateAccountReservationCommand> validator, IGlobalRulesService globalRulesService, IUnitOfWorkContext context)
        {
            _accountReservationService = accountReservationService;
            _validator = validator;
            _globalRulesService = globalRulesService;
            _context = context;
        }

        public async Task<CreateAccountReservationResult> Handle(CreateAccountReservationCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation", validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var globalRule = await _globalRulesService.CheckReservationAgainstRules(request);

            if (globalRule != null)
            {
                return new CreateAccountReservationResult
                {
                    Reservation = null,
                    Rule = globalRule
                };
            }

            var reservation = await _accountReservationService.CreateAccountReservation(request);

            _context.AddEvent(() => new ReservationCreatedEvent(reservation.Id,
                reservation.AccountLegalEntityId, 
                reservation.AccountLegalEntityName,
                reservation.Course?.CourseId, 
                reservation.StartDate, 
                reservation.Course?.Title, 
                reservation.ExpiryDate, 
                reservation.CreatedDate,
                reservation.AccountId));

            return new CreateAccountReservationResult
            {
                Reservation = reservation
            };
        }
    }
}
