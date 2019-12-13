using System;

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
        public virtual DateTime? CurrentDateTime { get; set; }
        public string QueueMonitorItems { get; set; }
        public string ElasticSearchUsername { get; set; }
        public string ElasticSearchPassword { get; set; }
        public string ElasticSearchServerUrl { get; set; }
    }
}
