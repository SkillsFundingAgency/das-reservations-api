using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetAvailableDatesQueryHandler(
        IValidator<GetAvailableDatesQuery> validator,
        IAccountLegalEntitiesService accountLegalEntitiesService,
        IAvailableDatesService availableDatesService)
        : IRequestHandler<GetAvailableDatesQuery, GetAvailableDatesResult>
    {
        private readonly IAccountLegalEntitiesService _accountLegalEntitiesService = accountLegalEntitiesService;

        public async Task<GetAvailableDatesResult> Handle(GetAvailableDatesQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid())
                throw new ArgumentException(
                    "The following parameters have failed validation",
                    validationResult.ValidationDictionary.Select(pair => pair.Key).Aggregate((item1, item2) => $"{item1}, {item2}"));

            var availableDates = availableDatesService.GetAvailableDates();

            return new GetAvailableDatesResult
            {
                AvailableDates = availableDates
            };
        }
    }
}
