using System;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Infrastructure.Configuration
{
    public class CurrentDateTime : ICurrentDateTime
    {
        private readonly DateTime? _currentDateTime;

        public CurrentDateTime(DateTime? currentDateTime = null)
        {
            _currentDateTime = currentDateTime;
        }

        public DateTime GetDate()
        {
            if (_currentDateTime.HasValue)
            {
                return _currentDateTime.Value;
            }

            return DateTime.UtcNow;
        }
    }
}
