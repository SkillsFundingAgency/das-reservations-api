using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<AvailableDateStartWindow> GetAvailableDates(long accountId)
        {
            var eoiAccounts = _configuration.EoiAccountIds.Split(',');

            if (eoiAccounts.Contains(accountId.ToString()))
            {
                return new EoiAvailableDates(
                    _configuration.EoiNumberOfAvailableDates,
                    _configuration.EoiAvailableDatesMinDate,
                    _configuration.EoiAvailableDatesMaxDate)
                    .Dates;
            }

            return new AvailableDates(
                _configuration.NumberOfAvailableDates,
                _configuration.AvailableDatesMinDate, 
                _configuration.AvailableDatesMaxDate)
                .Dates;
        }
    }
}
