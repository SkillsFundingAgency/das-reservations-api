using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Queries
{
    public class GetAvailableDatesResult
    {
        public IEnumerable<AvailableDateStartWindow> AvailableDates { get; set; }
    }
}