using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation
{
    public class CreateAccountReservationCommandHandler : IRequestHandler<CreateAccountReservationCommand, CreateAccountReservationResult>
    {
        private readonly IAccountReservationService _accountReservationService;
        private readonly IValidator<CreateAccountReservationCommand> _validator;
        private readonly IGlobalRulesService _globalRulesService;
        private readonly IUnitOfWorkContext _context;
        private readonly IAccountLegalEntitiesService _accountLegalEntitiesService;

        public CreateAccountReservationCommandHandler(IAccountReservationService accountReservationService,
            IValidator<CreateAccountReservationCommand> validator, IGlobalRulesService globalRulesService,
            IUnitOfWorkContext context, IAccountLegalEntitiesService accountLegalEntitiesService)
        {
            _accountReservationService = accountReservationService;
            _validator = validator;
            _globalRulesService = globalRulesService;
            _context = context;
            _accountLegalEntitiesService = accountLegalEntitiesService;
        }

        public async Task<CreateAccountReservationResult> Handle(CreateAccountReservationCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException(
                    "The following parameters have failed validation", 
                    validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var globalRule = await _globalRulesService.CheckReservationAgainstRules(request);

            if (globalRule != null)
            {
                var shouldReturnError = false;
                switch (globalRule.Restriction)
                {
                    case AccountRestriction.All:
                    case AccountRestriction.Account:
                        shouldReturnError = true;
                        break;
                    case AccountRestriction.Levy:
                        if (request.IsLevyAccount)
                        {
                            shouldReturnError = true;
                        }

                        break;
                    case AccountRestriction.NonLevy:
                        if (!request.IsLevyAccount)
                        {
                            shouldReturnError = true;
                        }

                        break;
                }

                if (shouldReturnError)
                {
                    return new CreateAccountReservationResult
                    {
                        Reservation = null,
                        Rule = globalRule
                    };
                }
            }

            var accountLegalEntity =
                await _accountLegalEntitiesService.GetAccountLegalEntity(request.AccountLegalEntityId);

            if (request.IsLevyAccount)
            {
                request.AccountLegalEntityName = accountLegalEntity.AccountLegalEntityName;
            } 
            else if (!accountLegalEntity.IsLevy && !accountLegalEntity.AgreementSigned)
            {
                return new CreateAccountReservationResult
                {
                    Reservation = null,
                    Rule = null,
                    AgreementSigned = false
                };
            }

            var reservation = await _accountReservationService.CreateAccountReservation(request);
            
            _context.AddEvent(() =>
            {
                var startDate = DateTime.MinValue;
                if (reservation.StartDate.HasValue)
                {
                    startDate = reservation.StartDate.Value;
                }
                var expiryDate = DateTime.MinValue;
                if (reservation.ExpiryDate.HasValue)
                {
                    expiryDate = reservation.ExpiryDate.Value;
                }

                return new ReservationCreatedEvent(reservation.Id,
                    reservation.AccountId,
                    reservation.AccountLegalEntityId,
                    reservation.AccountLegalEntityName,
                    startDate,
                    expiryDate,
                    reservation.CreatedDate,
                    reservation.Course?.CourseId,
                    reservation.Course?.Title,
                    reservation.Course?.Level,
                    reservation.ProviderId
                    );
            });

            return new CreateAccountReservationResult
            {
                Reservation = reservation,
                AgreementSigned = true
            };
        }
    }
}
