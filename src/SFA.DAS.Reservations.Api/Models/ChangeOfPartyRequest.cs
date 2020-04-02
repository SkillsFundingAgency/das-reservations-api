using System;

namespace SFA.DAS.Reservations.Api.Models
{
    public class ChangeOfPartyRequest
    {
        public Guid ReservationId { get; set; }
        public long? AccountLegalEntityId { get; set; }
        public uint? ProviderId { get; set; }
    }
}