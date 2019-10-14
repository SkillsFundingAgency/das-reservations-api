using System;

namespace SFA.DAS.Reservations.Messages
{
    public class ReservationDeletedEvent
    {
        public ReservationDeletedEvent(Guid id)
        {
            Id = id;

        }

        public Guid Id { get; set; }
    }
}
