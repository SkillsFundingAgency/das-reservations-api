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
    public class GetAvailableDatesQueryHandler : IRequestHandler<GetAvailableDatesQuery, GetAvailableDatesResult>
    {
        private readonly IValidator<GetAvailableDatesQuery> _validator;
        private readonly IAccountLegalEntitiesService _accountLegalEntitiesService;
        private readonly IAvailableDatesService _availableDatesService;

        public GetAvailableDatesQueryHandler(
            IValidator<GetAvailableDatesQuery> validator,
            IAccountLegalEntitiesService accountLegalEntitiesService,
            IAvailableDatesService availableDatesService)
        {
            _validator = validator;
            _accountLegalEntitiesService = accountLegalEntitiesService;
            _availableDatesService = availableDatesService;
        }

        public async Task<GetAvailableDatesResult> Handle(GetAvailableDatesQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid())
                throw new ArgumentException(
                    "The following parameters have failed validation",
                    validationResult.ValidationDictionary.Select(pair => pair.Key).Aggregate((item1, item2) => $"{item1}, {item2}"));

            var availableDates = _availableDatesService.GetAvailableDates();

            return new GetAvailableDatesResult
            {
                AvailableDates = availableDates
            };
        }
    }
}
