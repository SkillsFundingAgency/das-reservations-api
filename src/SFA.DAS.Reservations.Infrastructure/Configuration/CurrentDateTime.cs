using System;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Infrastructure.Configuration
{
    public class CurrentDateTime(DateTime? currentDateTime = null) : ICurrentDateTime
    {
        public DateTime GetDate()
        {
            if (currentDateTime.HasValue)
            {
                return currentDateTime.Value;
            }

            return DateTime.UtcNow;
        }
    }
}
