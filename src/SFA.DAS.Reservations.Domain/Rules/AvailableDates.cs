using System;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public class AvailableDates
    {
        private const int DefaultExpiryMonths = 6;

        public AvailableDates(int expiryPeriodInMonths =6, DateTime? minStartDate = null, DateTime? maxStartDate = null)
        {
            var expiryMonths = expiryPeriodInMonths == 0 ?
                DefaultExpiryMonths : expiryPeriodInMonths;

            if (expiryMonths > 12)
            {
                expiryMonths = 12;
            }

            var startDate = minStartDate ?? DateTime.UtcNow;

            var availableDates = new List<DateTime>
            {
                new DateTime(startDate.Year, startDate.Month, 1)
            };

            for (var i = 1; i < expiryMonths; i++)
            {
                var monthToAdd = startDate.AddMonths(i);
                availableDates.Add(new DateTime(monthToAdd.Year, monthToAdd.Month, 1));

                if (maxStartDate.HasValue &&
                    monthToAdd >= maxStartDate)
                {
                    break;
                }
            }

            Dates = availableDates;
        }

        public IList<DateTime> Dates { get; }
    }
}
