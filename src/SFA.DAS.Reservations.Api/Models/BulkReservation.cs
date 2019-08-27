namespace SFA.DAS.Reservations.Api.Models
{
    public class BulkReservation
    {
        public uint Count { get; set; }
        public long? TransferSenderId { get; set; }
    }
}