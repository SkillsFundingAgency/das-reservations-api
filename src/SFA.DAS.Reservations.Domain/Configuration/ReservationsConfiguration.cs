using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Reservations.Domain.Configuration
{
    public class ReservationsConfiguration
    {
        public virtual int ExpiryPeriodInMonths { get; set; }
        public virtual string ConnectionString { get; set; }
        public virtual DateTime? ExpiryPeriodMinDate { get; set; }
        public virtual DateTime? ExpiryPeriodMaxDate { get; set; }
    }
}
