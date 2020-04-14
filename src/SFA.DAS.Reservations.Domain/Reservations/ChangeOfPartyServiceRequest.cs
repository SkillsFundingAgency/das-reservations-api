using System;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class ChangeOfPartyServiceRequest
    {
        public Guid ReservationId { get; set; }
        public long? AccountLegalEntityId { get; set; }
        public uint? ProviderId { get; set; }
    }
}