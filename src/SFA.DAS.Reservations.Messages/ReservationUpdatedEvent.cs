using System;

namespace SFA.DAS.Reservations.Messages
{
    public class ReservationUpdatedEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reservationId">The ID of the reservation that is being updated</param>
        /// <param name="reservationStatus">The status of the current reservation. Choices are:
        ///  Pending = 0,
        ///  Confirmed = 1,
        ///  Completed = 2,
        ///  Deleted = 3</param>
        public ReservationUpdatedEvent(Guid reservationId, short reservationStatus)
        {
            Id = reservationId;
            Status = reservationStatus;
        }

        public Guid Id { get; set; }
        public short Status { get; set; }
    }
}
