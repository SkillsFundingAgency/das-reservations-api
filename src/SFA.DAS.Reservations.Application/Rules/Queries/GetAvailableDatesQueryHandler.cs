using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetAvailableDatesQueryHandler : IRequestHandler<GetAvailableDatesQuery, GetAvailableDatesResult>
    {
        private readonly IAccountLegalEntitiesService _accountLegalEntitiesService;
        private readonly IAvailableDatesService _availableDatesService;
        
        public GetAvailableDatesQueryHandler(
            IAccountLegalEntitiesService accountLegalEntitiesService,
            IAvailableDatesService availableDatesService)
        {
            _accountLegalEntitiesService = accountLegalEntitiesService;
            _availableDatesService = availableDatesService;
        }

        public async Task<GetAvailableDatesResult> Handle(GetAvailableDatesQuery request, CancellationToken cancellationToken)
        {
            var accountLegalEntity = await _accountLegalEntitiesService.GetAccountLegalEntity(request.AccountLegalEntityId);

            var availableDates = _availableDatesService.GetAvailableDates(accountLegalEntity.AccountId);
            
            return new GetAvailableDatesResult
            {
                AvailableDates = availableDates
            };
        }
    }
}
