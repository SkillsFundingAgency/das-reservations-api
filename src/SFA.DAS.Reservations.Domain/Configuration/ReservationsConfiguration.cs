using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Reservations.Domain.Configuration
{
    public class ReservationsConfiguration
    {
        public virtual int ExpiryPeriodInMonths { get; set; }
        public virtual string ConnectionString { get; set; }
        public virtual DateTime? AvailableDatesMinDate { get; set; }
        public virtual DateTime? AvailableDatesMaxDate { get; set; }
        public virtual int NumberOfAvailableDates { get; set; }
		public virtual int MaxNumberOfReservations { get; set; }
        public string NServiceBusConnectionString { get; set; }
    }
}
