﻿using System;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public class EoiAvailableDates
    {
        public EoiAvailableDates(
            int? numberOfAvailableDates = 6, 
            DateTime? availableDatesMinDate = null, 
            DateTime? availableDatesMaxDate = null)
        {
            var defaultNumberOfDates = numberOfAvailableDates ?? 6;
            var defaultStartDate = availableDatesMinDate ?? new DateTime(2019, 8, 1);
            var defaultEndDate = availableDatesMaxDate ?? new DateTime(2019, 12, 1);

            var availableDates = new AvailableDates(defaultNumberOfDates, defaultStartDate, defaultEndDate);

            Dates = availableDates.Dates;
        }

        public IEnumerable<AvailableDateStartWindow> Dates { get; }
    }
}