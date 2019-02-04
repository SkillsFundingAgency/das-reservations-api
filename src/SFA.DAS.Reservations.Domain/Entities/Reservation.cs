using System;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public long AccountId { get; set; }
        public bool IsLevyAccount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public short Status { get; set; }
    }
}