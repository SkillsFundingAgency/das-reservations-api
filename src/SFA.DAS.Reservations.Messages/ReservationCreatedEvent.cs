using System;

namespace SFA.DAS.Reservations.Messages
{
    public class ReservationCreatedEvent(
        Guid id,
        long accountId,
        long accountLegalEntityId,
        string accountLegalEntityName,
        DateTime startDate,
        DateTime endDate,
        DateTime createdDate,
        string courseId,
        string courseName,
        string courseLevel,
        uint? providerId)
    {
        public Guid Id { get; set; } = id;
        public long AccountId { get; set; } = accountId;
        public long AccountLegalEntityId { get; set; } = accountLegalEntityId;
        public string AccountLegalEntityName { get; set; } = accountLegalEntityName;
        public DateTime StartDate { get; set; } = startDate;
        public DateTime EndDate { get; set; } = endDate;
        public DateTime CreatedDate { get; set; } = createdDate;
        public string CourseId { get; set; } = courseId;
        public string CourseName { get; set; } = courseName;
        public string CourseLevel { get; set; } = courseLevel;
        public uint? ProviderId { get; set; } = providerId;
    }
}
