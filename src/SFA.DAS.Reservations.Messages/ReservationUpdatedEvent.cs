using System;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Messages
{
    public class ReservationUpdatedEvent
    {
        public ReservationUpdatedEvent(Guid reservationId, ReservationStatus status)
        {
            Id = reservationId;
            Status = status;
        }

        public Guid Id { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
