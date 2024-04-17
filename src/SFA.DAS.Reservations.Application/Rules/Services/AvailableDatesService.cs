using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Services
{
    public class AvailableDatesService : IAvailableDatesService
    {
        private readonly ICurrentDateTime _currentDateTime;
        private readonly ReservationsConfiguration _configuration;

        public AvailableDatesService(IOptions<ReservationsConfiguration> options, ICurrentDateTime currentDateTime)
        {
            _currentDateTime = currentDateTime;
            _configuration = options.Value;
        }

        public IEnumerable<AvailableDateStartWindow> GetAvailableDates()
        {
            return new AvailableDates(
                _currentDateTime.GetDate(),
                _configuration.NumberOfAvailableDates,
                _configuration.AvailableDatesMinDate,
                _configuration.AvailableDatesMaxDate)
                .Dates;
        }
    }
}
