using System;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class ReservationDates
    {
        public DateTime TrainingStartDate { get; set; }
        public DateTime ReservationStartDate { get; set; }
        public DateTime ReservationExpiryDate { get; set; }
        public DateTime ReservationCreatedDate { get; set; }
    }
}