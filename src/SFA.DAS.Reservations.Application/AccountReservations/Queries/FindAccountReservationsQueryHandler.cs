using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class FindAccountReservationsQueryHandler :  IRequestHandler<FindAccountReservationsQuery, FindAccountReservationsResult>
    {
        private readonly IAccountReservationService _service;
        private readonly IValidator<FindAccountReservationsQuery> _validator;

        public FindAccountReservationsQueryHandler(IAccountReservationService service,
            IValidator<FindAccountReservationsQuery> validator)
        {
            _service = service;
            _validator = validator;
        }
        public async Task<FindAccountReservationsResult> Handle(FindAccountReservationsQuery query, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(query);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation",
                    validationResult.ValidationDictionary.Select(c=>c.Key)
                                                                 .Aggregate((item1, item2)=> item1 +", "+item2));
            }

            var reservations = await _service.FindReservations(query.ProviderId, query.SearchTerm);

            return new FindAccountReservationsResult
            {
                Reservations = reservations
            };
        }
    }
}
