using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class ValidateReservationQueryHandler : IRequestHandler<ValidateReservationQuery, ValidateReservationResponse>
    {
        private readonly IAccountReservationService _reservationService;
        private readonly IValidator<ValidateReservationQuery> _validator;

        public ValidateReservationQueryHandler(IAccountReservationService reservationService,
            IValidator<ValidateReservationQuery> validator)
        {
            _reservationService = reservationService;
            _validator = validator;
        }

        public async Task<ValidateReservationResponse> Handle(ValidateReservationQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation",validationResult.ValidationDictionary.Select(c=>c.Key).Aggregate((item1, item2)=> item1 +", "+item2));
            }

            var errors = new List<ReservationValidationError>();

            var reservation = await _reservationService.GetReservation(request.ReservationId);

            if (reservation.StartDate > request.TrainingStartDate)
            {
                errors.Add(new ReservationValidationError(nameof(request.TrainingStartDate), "Training start date must be after reservation start date"));
            }

            if (reservation.ExpiryDate < request.TrainingStartDate)
            {
                errors.Add(new ReservationValidationError(nameof(request.TrainingStartDate), "Training start date must be before reservation expiry date"));
            }

            if (reservation.Course.GetActiveRules().Any())
            {
                errors.Add(new ReservationValidationError(nameof(request.CourseId),"Selected course has restriction rules associated with it"));
            }

            return new ValidateReservationResponse
            {
                Errors = errors
            };
        }
    }
}
