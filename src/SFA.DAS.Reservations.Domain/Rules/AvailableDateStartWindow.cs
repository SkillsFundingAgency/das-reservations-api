using System;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public class AvailableDateStartWindow
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}