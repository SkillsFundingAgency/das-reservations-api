using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class GetAccountReservationsQueryHandler(
        IValidator<GetAccountReservationsQuery> validator,
        IAccountReservationService service)
        : IRequestHandler<GetAccountReservationsQuery, GetAccountReservationsResult>
    {
        public async Task<GetAccountReservationsResult> Handle(GetAccountReservationsQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation",validationResult.ValidationDictionary.Select(c=>c.Key).Aggregate((item1, item2)=> item1 +", "+item2));
            }

            var reservations = await service.GetAccountReservations(request.AccountId);

            return new GetAccountReservationsResult{Reservations = reservations};
        }
    }
}
