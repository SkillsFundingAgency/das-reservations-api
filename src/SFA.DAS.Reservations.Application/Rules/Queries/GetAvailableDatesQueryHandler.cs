using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetAvailableDatesQueryHandler : IRequestHandler<GetAvailableDatesQuery, GetAvailableDatesResult>
    {
        private readonly IAvailableDatesService _availableDatesService;
        
        public GetAvailableDatesQueryHandler(IAvailableDatesService availableDatesService)
        {
            _availableDatesService = availableDatesService;
        }

        public async Task<GetAvailableDatesResult> Handle(GetAvailableDatesQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;//todo: remove once _availableDatesService is awaitable
            // todo: get account id
            // todo: check if account is eoi

            var availableDates = _availableDatesService.GetAvailableDates(request.AccountLegalEntityId);
            
            return new GetAvailableDatesResult
            {
                AvailableDates = availableDates
            };
        }
    }
}
