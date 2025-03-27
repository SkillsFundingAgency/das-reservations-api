using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class FindAccountReservationsQueryHandler(
        IAccountReservationService service,
        IValidator<FindAccountReservationsQuery> validator)
        : IRequestHandler<FindAccountReservationsQuery, FindAccountReservationsResult>
    {
        public async Task<FindAccountReservationsResult> Handle(FindAccountReservationsQuery query, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(query);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation",
                    validationResult.ValidationDictionary.Select(c=>c.Key)
                                                                 .Aggregate((item1, item2)=> item1 +", "+item2));
            }

            var result = await service.FindReservations(
                query.ProviderId,
                query.SearchTerm,
                query.PageNumber,
                query.PageItemCount,
                query.SelectedFilters);

            return new FindAccountReservationsResult
            {
                Reservations = result.Reservations,
                NumberOfRecordsFound = result.TotalReservations,
                Filters = result.Filters,
                TotalReservationsForProvider = result.TotalReservationsForProvider
            };
        }
    }
}
