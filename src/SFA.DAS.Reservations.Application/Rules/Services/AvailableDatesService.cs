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

        public IList<AvailableDateStartWindow> GetAvailableDates(long accountLegalEntity)
        {
            //todo: get account for this ale
            //todo: check if account is eoi

            var availableDates = new AvailableDates(
                _configuration.NumberOfAvailableDates,
                _configuration.AvailableDatesMinDate, 
                _configuration.AvailableDatesMaxDate);
            return availableDates.Dates;
        }
    }
}
