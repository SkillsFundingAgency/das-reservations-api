using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class GetReservationQueryHandler(
        IValidator<GetReservationQuery> validator,
        IAccountReservationService accountReservationService)
        : IRequestHandler<GetReservationQuery, GetReservationResponse>
    {
        public async Task<GetReservationResponse> Handle(GetReservationQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation", validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var reservation = await accountReservationService.GetReservation(request.Id);

            if (reservation == null)
            {
                return null;
            }

            return new GetReservationResponse
            {
                Reservation = reservation
            };
        }
    }
}
