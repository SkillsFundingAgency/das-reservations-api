using System;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetAvailableDatesResult
    {
        public IList<DateTime> AvailableDates { get; set; }
    }
}