using System;
using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetAvailableDatesResult
    {
        public IList<AvailableDateStartWindow> AvailableDates { get; set; }
    }
}