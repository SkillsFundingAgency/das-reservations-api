namespace SFA.DAS.Reservations.Api.Models
{
    public class ChangeOfPartyRequest
    {
        public long? AccountLegalEntityId { get; set; }
        public uint? ProviderId { get; set; }
    }
}