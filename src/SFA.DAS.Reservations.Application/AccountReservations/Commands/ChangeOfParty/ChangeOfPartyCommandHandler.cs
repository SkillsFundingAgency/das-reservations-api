using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.ChangeOfParty
{
    public class ChangeOfPartyCommandHandler : IRequestHandler<ChangeOfPartyCommand, ChangeOfPartyResult>
    {
        private readonly IValidator<ChangeOfPartyCommand> _validator;
        private readonly IAccountReservationService _reservationService;

        public ChangeOfPartyCommandHandler(
            IValidator<ChangeOfPartyCommand> validator,
            IAccountReservationService reservationService)
        {
            _validator = validator;
            _reservationService = reservationService;
        }
        
        public async Task<ChangeOfPartyResult> Handle(ChangeOfPartyCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException(
                    "The following parameters have failed validation", 
                    validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var clonedReservationId = await _reservationService.ChangeOfParty(new ChangeOfPartyServiceRequest
            {
                ReservationId = request.ReservationId,
                AccountLegalEntityId = request.AccountLegalEntityId,
                ProviderId = request.ProviderId
            });

            return new ChangeOfPartyResult {ReservationId = clonedReservationId};
        }
    }
}