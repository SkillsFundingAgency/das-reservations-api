using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Reservations.Domain.Configuration
{
    public class ReservationConfiguration
    {
        public virtual int ExpiryPeriodInMonths { get; set; }
    }
}
