using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class GetReservationQueryHandler : IRequestHandler<GetReservationQuery,GetReservationResponse>
    {
        private readonly IValidator<GetReservationQuery> _validator;
        private readonly IAccountReservationService _accountReservationService;

        public GetReservationQueryHandler(IValidator<GetReservationQuery> validator, IAccountReservationService accountReservationService)
        {
            _validator = validator;
            _accountReservationService = accountReservationService;
        }

        public async Task<GetReservationResponse> Handle(GetReservationQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation", validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var reservation = await _accountReservationService.GetReservation(request.Id);

            return new GetReservationResponse
            {
                Reservation = reservation
            };
        }
    }
}
