using System;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public class EoiAvailableDates
    {
        public EoiAvailableDates(
            int numberOfAvailableDates = 6, 
            DateTime? availableDatesMinDate = null, 
            DateTime? availableDatesMaxDate = null)
        {
            Dates = new List<AvailableDateStartWindow>();
        }

        public IEnumerable<AvailableDateStartWindow> Dates { get; }
    }
}