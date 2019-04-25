using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;
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
            var availableDates = _availableDatesService.GetAvailableDates();

            return new GetAvailableDatesResult
            {
                AvailableDates = availableDates
            };
        }
    }
}
