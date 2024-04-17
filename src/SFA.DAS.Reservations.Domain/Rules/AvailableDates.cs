using System;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public class AvailableDates
    {
        private const int DefaultExpiryMonths = 6;

        public AvailableDates(
            DateTime dateTimeNow,
            int? expiryPeriodInMonths = 6, 
            DateTime? minStartDate = null,
            DateTime? maxStartDate = null
            )
        {         
            var expiryMonths = expiryPeriodInMonths == 0 ? DefaultExpiryMonths : expiryPeriodInMonths;

            if (expiryMonths > 12)
            {
                expiryMonths = 12;
            }
           
            var startDate = minStartDate ?? dateTimeNow;            
            var twoMonthsFromNow = startDate.AddMonths(2);
            var lastDayOfTheMonth = DateTime.DaysInMonth(twoMonthsFromNow.Year, twoMonthsFromNow.Month);

            if (maxStartDate.HasValue && startDate > maxStartDate)
            {
                Dates =  new List<AvailableDateStartWindow>();
                return;
            }
            
            var availableDates = new List<AvailableDateStartWindow>
            {               
                new() {
                    StartDate = new DateTime(startDate.Year, startDate.Month, 1),
                    EndDate = new DateTime(twoMonthsFromNow.Year, twoMonthsFromNow.Month, lastDayOfTheMonth)
                }
            };

            for (var i = 1; i < expiryMonths; i++)
            {
                if (maxStartDate.HasValue && 
                    new DateTime(maxStartDate.Value.Year, maxStartDate.Value.Month, 1) <= dateTimeNow)
                    break;

                var monthToAdd = startDate.AddMonths(i);
                twoMonthsFromNow = monthToAdd.AddMonths(2);
                lastDayOfTheMonth = DateTime.DaysInMonth(twoMonthsFromNow.Year, twoMonthsFromNow.Month);
                availableDates.Add(new AvailableDateStartWindow
                {
                    StartDate = new DateTime(monthToAdd.Year, monthToAdd.Month, 1),
                    EndDate = new DateTime(twoMonthsFromNow.Year, twoMonthsFromNow.Month, lastDayOfTheMonth)
                });

                if (maxStartDate.HasValue &&
                    monthToAdd >= maxStartDate)
                {
                    break;
                }
            }

            Dates = availableDates;
        }

        public IList<AvailableDateStartWindow> Dates { get; }
    }
}