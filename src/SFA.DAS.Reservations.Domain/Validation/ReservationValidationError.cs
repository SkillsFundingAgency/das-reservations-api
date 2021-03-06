﻿namespace SFA.DAS.Reservations.Domain.Validation
{
    public class ReservationValidationError
    {
        public string PropertyName { get; }
        public string Reason { get;  }

        public ReservationValidationError(string propertyName, string reason)
        {
            PropertyName = propertyName;
            Reason = reason;
        }
    }
}
