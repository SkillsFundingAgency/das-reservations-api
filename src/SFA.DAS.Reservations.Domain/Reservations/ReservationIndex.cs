using System;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class ReservationIndex
    {
        public string Id { get; set; }
        public Guid ReservationId { get; set; }
        public long AccountId { get; set; }
        public bool IsLevyAccount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public short Status { get; set; }
        public string CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int? CourseLevel { get; set; }
        public string CourseName { get; set; }
        public long AccountLegalEntityId { get; set; }
        public uint? ProviderId { get; set; }
        public string AccountLegalEntityName { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public Guid? UserId { get; set; }
        public uint IndexedProviderId { get; set; }
    }
}
