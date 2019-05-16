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
        
        public virtual string EoiAccountIds { get; set; }
        public virtual int? EoiNumberOfAvailableDates { get; set; }
        public virtual DateTime? EoiAvailableDatesMinDate { get; set; }
        public virtual DateTime? EoiAvailableDatesMaxDate { get; set; }
    }
}
