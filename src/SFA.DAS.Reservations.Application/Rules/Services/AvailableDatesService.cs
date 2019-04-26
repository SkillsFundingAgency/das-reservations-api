using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Services
{
    public class AvailableDatesService : IAvailableDatesService
    {
        private readonly ReservationsConfiguration _configuration;

        public AvailableDatesService(IOptions<ReservationsConfiguration> options)
        {
            _configuration = options.Value;
        }

        public IList<AvailableDateStartWindow> GetAvailableDates()
        {
            var availableDates = new AvailableDates(_configuration.ExpiryPeriodInMonths,
                _configuration.ExpiryPeriodMinDate, _configuration.ExpiryPeriodMaxDate);
            return availableDates.Dates;
        }
    }
}
